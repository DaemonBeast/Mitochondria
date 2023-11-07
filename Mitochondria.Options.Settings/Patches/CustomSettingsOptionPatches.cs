using System.Reflection;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Mitochondria.Options.Settings.Framework.Managers;
using UnityEngine;

namespace Mitochondria.Options.Settings.Patches;

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
                !CustomSettingsOptionManager.Instance.TryGet(gameMode, out var customSettingsOptions))
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

    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.FixedUpdate))]
    public static class NumberOptionUpdateTextPatch
    {
        public static bool Prefix(NumberOption __instance)
        {
            var value = __instance.Value;

            if (Mathf.Approximately(__instance.oldValue, value))
            {
                return false;
            }

            if (!CustomSettingsOptionManager.Instance
                    .TryGetSettingsOption(__instance.gameObject, out var settingsOption))
            {
                return true;
            }

            __instance.oldValue = value;

            __instance.ValueText.text = settingsOption.BoxedCustomOption.ValueString;

            return false;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
    public static class NumberOptionTextPatch
    {
        public static void Postfix(NumberOption __instance)
        {
            if (CustomSettingsOptionManager.Instance
                .TryGetSettingsOption(__instance.gameObject, out var settingsOption))
            {
                __instance.ValueText.text = settingsOption.BoxedCustomOption.ValueString;
            }
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.FixedUpdate))]
    public static class StringOptionUpdateTextPatch
    {
        public static bool Prefix(StringOption __instance)
        {
            var value = __instance.Value;

            if (__instance.oldValue == value)
            {
                return false;
            }

            if (!CustomSettingsOptionManager.Instance
                    .TryGetSettingsOption(__instance.gameObject, out var settingsOption))
            {
                return true;
            }

            __instance.oldValue = value;

            __instance.ValueText.text = settingsOption.BoxedCustomOption.ValueString;

            return false;
        }
    }

    [HarmonyPatch]
    public static class SetRecommendationsPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            return AccessTools.GetTypesFromAssembly(typeof(IGameOptions).Assembly)
                .SelectMany(t => t.GetMethods())
                .Where(m => m.Name == nameof(IGameOptions.SetRecommendations) &&
                            m.DeclaringType?.Name != nameof(IGameOptions) &&
                            Il2CppType.Of<IGameOptions>().IsAssignableFrom(Il2CppType.From(m.DeclaringType)));
        }

        public static void Prefix()
        {
            CustomSettingsOptionManager.Instance.ResetAllOptions(GameOptionsManager.Instance.currentGameMode);
        }
    }
}