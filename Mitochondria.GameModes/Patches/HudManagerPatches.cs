using HarmonyLib;

namespace Mitochondria.GameModes.Patches;

internal static class HudManagerPatches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class TaskPanelPatch
    {
        public static void Postfix(HudManager __instance)
        {
            if (GameManager.Instance == null ||
                !GameManager.Instance.TryGetComponent<CustomGameModeBehaviour>(
                    out var customGameModeBehaviour)) return;

            __instance.TaskStuff.SetActive(customGameModeBehaviour.CustomGameMode.Configuration.Tasks.Enabled);
        }
    }
}
