using AmongUs.Data;
using HarmonyLib;
using Mitochondria.GameModes.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace Mitochondria.GameModes.Patches;

internal static class GameStartManagerPatches
{
    public static void Reset()
    {
        if (!GameStartManager.InstanceExists) return;
        var gameStartManager = GameStartManager.Instance;

        if (!CustomGameModeManager.GameModes.TryGetValue(
                DataManager.Settings.Multiplayer.LastPlayedGameMode,
                out var customGameMode))
        {
            gameStartManager.MinPlayers = 4;
            return;
        }

        gameStartManager.MinPlayers = customGameMode.Configuration.Players.MinPlayers;
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public static class MinPlayersPatch
    {
        public static void Postfix()
            => Reset();
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
    public static class BeginGamePatch
    {
        public static bool Prefix(GameStartManager __instance)
        {
            if (!CustomGameModeUtilities.TryGetActiveGameMode(out _) ||
                __instance.startState != GameStartManager.StartingStates.NotStarting ||
                GameData.Instance.PlayerCount < __instance.MinPlayers) return true;

            __instance.ReallyBegin(false);

            return false;
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class CanStartPatch
    {
        public static void Postfix(GameStartManager __instance)
        {
            if (AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame ||
                !CustomGameModeUtilities.TryGetActiveGameModeFlow(out var customGameModeFlow)) return;

            var color = __instance.LastPlayerCount < __instance.MinPlayers
                ? new Color(1, 0, 0)
                : __instance.LastPlayerCount > __instance.MinPlayers
                    ? new Color(0, 1, 0)
                    : __instance.MinPlayers == GameManager.Instance.LogicOptions.MaxPlayers
                        ? new Color(0, 1, 0)
                        : new Color(1, 1, 0);

            var playerCount = __instance.LastPlayerCount;
            var maxPlayers = GameManager.Instance.LogicOptions.MaxPlayers;
            __instance.PlayerCounter.text = $"<color=#{color.ToHtmlStringRGBA()}>{playerCount}/{maxPlayers}</color>";

            var customCanStartGame = customGameModeFlow.CanStartGame();
            var canStartGame = playerCount >= __instance.MinPlayers && customCanStartGame.CanStartGame;

            __instance.StartButton.SetButtonEnableState(canStartGame);

            if (__instance.StartButtonGlyph != null)
            {
                __instance.StartButtonGlyph.SetColor(canStartGame ? Palette.EnabledColor : Palette.DisabledClear);
            }

            __instance.StartButton.ChangeButtonText(
                customCanStartGame.CanStartGame
                    ? TranslationController.Instance.GetString(
                        canStartGame ? StringNames.StartLabel : StringNames.WaitingForPlayers)
                    : customCanStartGame.Explanation);

            __instance.StartButton.buttonText.enableWordWrapping = true;
            __instance.StartButton.buttonText.alignment = TextAlignmentOptions.Center;

            __instance.GameStartTextClient.text = TranslationController.Instance.GetString(canStartGame
                ? StringNames.WaitingForHost
                : StringNames.WaitingForPlayers);

            if (DiscordManager.InstanceExists)
            {
                if (AmongUsClient.Instance.AmHost)
                {
                    DiscordManager.Instance.SetInLobbyHost(
                        __instance.LastPlayerCount,
                        GameManager.Instance.LogicOptions.MaxPlayers,
                        AmongUsClient.Instance.GameId);
                }
                else
                {
                    DiscordManager.Instance.SetInLobbyClient(
                        __instance.LastPlayerCount,
                        GameManager.Instance.LogicOptions.MaxPlayers,
                        AmongUsClient.Instance.GameId);
                }
            }
        }
    }
}
