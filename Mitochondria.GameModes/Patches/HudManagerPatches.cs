using HarmonyLib;
using Mitochondria.GameModes.Utilities;

namespace Mitochondria.GameModes.Patches;

internal static class HudManagerPatches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class TaskPanelPatch
    {
        public static void Postfix(HudManager __instance)
        {
            if (!CustomGameModeUtilities.TryGetActiveGameMode(out var customGameMode)) return;

            __instance.TaskStuff.SetActive(customGameMode.Configuration.Tasks.Enabled);
        }
    }
}
