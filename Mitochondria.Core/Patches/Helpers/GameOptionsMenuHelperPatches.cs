using HarmonyLib;
using Mitochondria.Core.Framework.Helpers;

namespace Mitochondria.Core.Patches.Helpers;

internal static class GameOptionsMenuHelperPatches
{
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
    public static class CurrentSetPatch
    {
        [HarmonyPriority(Priority.First)]
        public static void Prefix(GameOptionsMenu __instance)
        {
            GameOptionsMenuHelper.Current = __instance;
            
            GameOptionsMenuHelper.InvokeOnBeforeOpened();
        }

        [HarmonyPriority(Priority.First)]
        public static void Postfix()
        {
            GameOptionsMenuHelper.InvokeOnAfterOpened();
        }
    }
}