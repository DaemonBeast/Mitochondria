using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Mitochondria.GameModes.Utilities.Extensions;

namespace Mitochondria.GameModes.Patches;

internal static class GameManagerPatches
{
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.Awake))]
    public static class CustomGameManagerPatch
    {
        public static void Postfix(GameManager __instance)
        {
            if (!CustomGameModeManager.GameModes.ContainsKey(
                    DataManager.Settings.Multiplayer.LastPlayedGameMode)) return;

            __instance.gameObject.AddComponent<CustomGameModeBehaviour>();
        }
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.StartGame))]
    public static class StartGamePatch
    {
        public static void Postfix(GameManager __instance)
        {
            if (!__instance.TryGetActiveCustomGameModeFlow(out var customGameModeFlow)) return;

            __instance.StartCoroutine(customGameModeFlow.CoAfterGameStart().WrapToIl2Cpp());
        }
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.EndGame))]
    public static class EndGamePatch
    {
        public static void Postfix(GameManager __instance)
        {
            if (!__instance.TryGetActiveCustomGameModeFlow(out var customGameModeFlow)) return;

            customGameModeFlow.AfterGameEnd();
        }
    }
}
