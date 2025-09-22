using AmongUs.GameOptions;
using HarmonyLib;

namespace Mitochondria.GameModes.Patches;

internal static class GameOptionsManagerPatches
{
    [HarmonyPatch(typeof(GameOptionsManager), nameof(GameOptionsManager.SwitchGameMode))]
    public static class SwitchGameModePatch
    {
        public static bool Prefix(GameOptionsManager __instance, AmongUs.GameOptions.GameModes gameMode)
        {
            if (gameMode == AmongUs.GameOptions.GameModes.None || Enum.IsDefined(gameMode)) return true;

            SetCustomGameMode(__instance, gameMode);
            return false;
        }

        public static void Postfix()
        {
            GameStartManagerPatches.Reset();
        }

        private static void SetCustomGameMode(
            GameOptionsManager gameOptionsManager,
            AmongUs.GameOptions.GameModes gameMode)
        {
            if (!CustomGameModeManager.GameModes.TryGetValue(gameMode, out var customGameMode))
            {
                Error($"Cannot switch to unregistered game mode \"{gameMode}\".");
                return;
            }

            // TODO: Custom options?
            gameOptionsManager.currentHostOptions = gameOptionsManager.normalGameHostOptions.Cast<IGameOptions>();
            gameOptionsManager.currentSearchOptions = gameOptionsManager.normalGameSearchOptions.Cast<IGameOptions>();

            gameOptionsManager.currentGameOptions = gameOptionsManager.currentNormalGameOptions == null
                ? gameOptionsManager.normalGameHostOptions.Cast<IGameOptions>()
                : gameOptionsManager.currentNormalGameOptions.Cast<IGameOptions>();

            gameOptionsManager.currentGameMode = gameMode;

            Info($"Set game mode to \"{TranslationController.Instance.GetString(customGameMode.Name)}\".");
        }
    }
}
