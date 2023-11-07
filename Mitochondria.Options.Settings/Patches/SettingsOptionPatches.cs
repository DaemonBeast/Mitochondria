using HarmonyLib;
using Il2CppInterop.Runtime;
using Mitochondria.Core.Framework.GUI.Extensions;
using Mitochondria.Core.Framework.Prototypes;
using Mitochondria.Core.Framework.Utilities.Extensions;
using Mitochondria.Options.Settings.Framework.Gui;
using Mitochondria.Options.Settings.Framework.Managers;

namespace Mitochondria.Options.Settings.Patches;

internal static class SettingsOptionPatches
{
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
    public static class FlexPatch
    {
        [HarmonyPriority(Priority.VeryHigh)]
        public static void Prefix(GameOptionsMenu __instance)
        {
            var gameSettingMenu = GameSettingMenu.Instance;
            if (gameSettingMenu != null)
            {
                // TODO: Add `KeyValueOption` and `PlayerOption`?

                __instance
                    .GetComponentInChildren<NumberOption>(true)
                    .ThenIfNotNull(n => PrototypeManager.Instance.CloneAndSet(n));

                __instance
                    .GetComponentsInChildren<ToggleOption>(true)
                    .ElementAtOrDefault(1)
                    .ThenIfNotNull(t => PrototypeManager.Instance.CloneAndSet(t));

                gameSettingMenu
                    .GetComponentInChildren<StringOption>(true)
                    .ThenIfNotNull(s => PrototypeManager.Instance.CloneAndSet(s));
            }

            var orderAccumulator = 0;

            var settingsOptionElements = __instance
                .GetComponentsInChildren<OptionBehaviour>()
                .Select(o =>
                {
                    var guiElement = o.gameObject.SetGuiElement<SettingsOptionElement>();
                    guiElement.Order = orderAccumulator += 10;

                    return guiElement;
                });

            SettingsOptionManager.Instance.Container!.AddRange(settingsOptionElements);
        }
    }
    
    [HarmonyPatch(typeof(OptionsConsole), nameof(OptionsConsole.CanUse))]
    public static class NonHostGameOptionsMenuShowPatch
    {
        public static void Prefix(OptionsConsole __instance)
        {
            __instance.HostOnly = false;
        }
    }

    [HarmonyPatch(typeof(OptionBehaviour), nameof(OptionBehaviour.SetAsPlayer))]
    public static class NonHostGameOptionsMenuOptionsPatch
    {
        public static bool Prefix(OptionBehaviour __instance)
        {
            var isRoleOptionSetting = __instance.TryCast<RoleOptionSetting>() != null;
            
            var buttonGameObjects = __instance.gameObject
                .GetChildren()
                .SelectMany(g => g.GetComponentsInChildren<PassiveButton>())
                .Select(p => p.gameObject)
                .Where(g => g.name != "More Options");

            foreach (var buttonGameObject in buttonGameObjects)
            {
                buttonGameObject.SetActive(false);
            }

            if ((!CustomSettingsOptionManager.Instance
                     .TryGetSettingsOption(__instance.gameObject, out var settingsOption) ||
                 settingsOption.BoxedCustomOption.HostOnly) &&
                !isRoleOptionSetting &&
                __instance.TryGetComponent(Il2CppType.Of<PassiveButton>(), out var b))
            {
                var button = b.Cast<PassiveButton>();

                button.enabled = false;
            }
            
            return false;
        }
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Update))]
    public static class NonHostRoleOptionsMenuRefreshPatch
    {
        public static void Postfix(RolesSettingsMenu __instance)
        {
            var roleOptions = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions;
            
            foreach (var optionBehaviour in __instance.Children)
            {
                var roleOptionSetting = optionBehaviour.TryCast<RoleOptionSetting>();
                if (roleOptionSetting == null)
                {
                    continue;
                }
                
                roleOptionSetting.UpdateValuesAndText(roleOptions);
            }
        }
    }
}