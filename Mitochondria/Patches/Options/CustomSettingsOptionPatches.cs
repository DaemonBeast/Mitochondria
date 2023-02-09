using AmongUs.GameOptions;
using HarmonyLib;
using Mitochondria.Framework.Options.SettingsOptions;

namespace Mitochondria.Patches.Options;

internal static class CustomSettingsOptionPatches
{
    [HarmonyPatch(typeof(GameOptionsManager), nameof(GameOptionsManager.SwitchGameMode))]
    public static class IsDefaultsPatch
    {
        public static void Postfix(GameOptionsManager __instance, GameModes gameMode)
        {
            var gameOptions = __instance.GameHostOptions;

            if (gameOptions == null ||
                !gameOptions.TryGetBool(BoolOptionNames.IsDefaults, out var isDefaults) ||
                !isDefaults ||
                !CustomSettingsOptionManager.Instance.CustomSettingsOptions
                    .TryGetValue(gameMode, out var customSettingsOptions))
            {
                return;
            }

            foreach (var customSettingsOption in customSettingsOptions)
            {
                if (!customSettingsOption.BoxedCustomOption.HasDefaultValue())
                {
                    gameOptions.SetBool(BoolOptionNames.IsDefaults, false);
                }
            }
        }
    }
}