using HarmonyLib;
using Il2CppInterop.Runtime;
using Mitochondria.Framework.Cache;
using Mitochondria.Framework.Options.SettingsOptions;
using Mitochondria.Framework.Options.SettingsOptions.Providers;
using Mitochondria.Framework.UI.Extensions;
using Mitochondria.Framework.UI.Flex.SettingsOptions;
using Mitochondria.Framework.Utilities.Extensions;
using Reactor.Utilities.Extensions;
using Object = UnityEngine.Object;

namespace Mitochondria.Patches.Options;

internal static class SettingsOptionPatches
{
    public delegate void GameOptionsMenuOpenedHandler(GameOptionsMenu gameOptionsMenu);

    public static event GameOptionsMenuOpenedHandler? PreGameOptionsMenuOpened;
    public static event GameOptionsMenuOpenedHandler? PostGameOptionsMenuOpened;

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
    public static class FlexPatch
    {
        public static void Prefix(GameOptionsMenu __instance)
        {
            PreGameOptionsMenuOpened?.Invoke(__instance);
            
            // Note that there are multiple objects of type `GameOptionsMenu`
            // and the currently active one will be cached as appropriate
            MonoBehaviourProvider.Set(__instance);
            
            if (MonoBehaviourProvider.TryGet(out GameSettingMenu? gameSettingMenu))
            {
                // TODO: Add `KeyValueOption` and `PlayerOption`?
                
                var originalNumberOption = __instance.GetComponentInChildren<NumberOption>(true);
                var originalToggleOption = __instance.GetComponentsInChildren<ToggleOption>(true).ElementAtOrDefault(1);
                var originalStringOption = gameSettingMenu.GetComponentInChildren<StringOption>(true);

                // Could probably be somewhat abstracted, but that's too much effort
                
                if (originalNumberOption != null)
                {
                    var clone = Object.Instantiate(originalNumberOption.gameObject).DontDestroy();
                    clone.SetActive(false);

                    CloneSettingsOptionProviderHelper.NumberOptionTemplate = clone;
                }

                if (originalToggleOption != null)
                {
                    var clone = Object.Instantiate(originalToggleOption.gameObject).DontDestroy();
                    clone.SetActive(false);

                    CloneSettingsOptionProviderHelper.ToggleOptionTemplate = clone;
                }

                if (originalStringOption != null)
                {
                    var clone = Object.Instantiate(originalStringOption.gameObject).DontDestroy();
                    clone.SetActive(false);

                    CloneSettingsOptionProviderHelper.StringOptionTemplate = clone;
                }
            }

            var flex = (IEnumerable<SettingsOptionFlex>) __instance
                .GetComponentsInChildren<OptionBehaviour>()
                .Select(o => o.gameObject.AsFlex() as SettingsOptionFlex)
                .Where(f => f != null);

            SettingsOptionManager.Instance.FlexContainer.TryAddRange(flex);
        }

        public static void Postfix(GameOptionsMenu __instance)
        {
            PostGameOptionsMenuOpened?.Invoke(__instance);
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

            if ((!SettingsOptionManager.Instance.TryGetSettingsOption(__instance.gameObject, out var settingsOption) ||
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