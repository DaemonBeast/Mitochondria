using HarmonyLib;
using Il2CppInterop.Runtime;
using Mitochondria.Framework.Options.SettingsOptions;
using Mitochondria.Framework.Options.SettingsOptions.Managers;
using Mitochondria.Framework.UI.Extensions;
using Mitochondria.Framework.UI.Flex.SettingsOptions;
using Mitochondria.Framework.Utilities.Extensions;

namespace Mitochondria.Patches.Options;

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
                    .ThenIfNotNull(n => SettingsOptionPrototypeManager.Instance.CloneAndSet<NumberOption>(n));

                __instance
                    .GetComponentsInChildren<ToggleOption>(true)
                    .ElementAtOrDefault(1)
                    .ThenIfNotNull(t => SettingsOptionPrototypeManager.Instance.CloneAndSet<ToggleOption>(t));

                gameSettingMenu
                    .GetComponentInChildren<StringOption>(true)
                    .ThenIfNotNull(s => SettingsOptionPrototypeManager.Instance.CloneAndSet<StringOption>(s));
            }

            var flex = (IEnumerable<SettingsOptionFlex>) __instance
                .GetComponentsInChildren<OptionBehaviour>()
                .Select(o => o.gameObject.AsFlex() as SettingsOptionFlex)
                .Where(f => f != null);

            SettingsOptionManager.Instance.FlexContainer.TryAddRange(flex);
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