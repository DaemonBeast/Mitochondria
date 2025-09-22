using AmongUs.Data;
using HarmonyLib;

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
            if (GameManager.Instance == null ||
                !GameManager.Instance.TryGetComponent<CustomGameModeBehaviour>(out var customGameModeBehaviour) ||
                __instance.startState != GameStartManager.StartingStates.NotStarting ||
                GameData.Instance.PlayerCount < __instance.MinPlayers) return true;

            __instance.ReallyBegin(false);

            return false;
        }
    }
}
