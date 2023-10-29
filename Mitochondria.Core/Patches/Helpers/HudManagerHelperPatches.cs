using HarmonyLib;
using Mitochondria.Core.Framework.Helpers;

namespace Mitochondria.Core.Patches.Helpers;

public static class HudManagerHelperPatches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class TransformsSetPatch
    {
        [HarmonyPriority(Priority.First)]
        public static void Prefix(HudManager __instance)
        {
            HudManagerHelper.Transforms.BottomRightButtons = __instance.transform.Find("Buttons").Find("BottomRight");
        }
    }
}