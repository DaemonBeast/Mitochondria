using HarmonyLib;
using Mitochondria.Framework.Cache;
using Mitochondria.Framework.Options.SettingsOptions;
using Mitochondria.Framework.Options.SettingsOptions.Providers;
using Mitochondria.Framework.UI.Extensions;
using Mitochondria.Framework.UI.Flex.SettingsOptions;
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
        public static void Postfix(GameOptionsMenu __instance)
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

            PostGameOptionsMenuOpened?.Invoke(__instance);
        }
    }
}