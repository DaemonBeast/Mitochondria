using HarmonyLib;

namespace Mitochondria.GameModes.Patches;

internal static class LogicOptionsNormalPatches
{
    [HarmonyPatch(typeof(LogicOptionsNormal), nameof(LogicOptionsNormal.GetTaskBarMode))]
    public static class TaskBarModePatch
    {
        public static bool Prefix(LogicOptionsNormal __instance, ref TaskBarMode __result)
        {
            if (!__instance.Manager.TryGetComponent<CustomGameModeBehaviour>(
                    out var customGameModeBehaviour)) return true;

            if (!customGameModeBehaviour.CustomGameMode.Configuration.Tasks.Enabled)
            {
                __result = TaskBarMode.Invisible;
            }

            return false;
        }
    }
}
