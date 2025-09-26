using HarmonyLib;

namespace Mitochondria.GameModes.Patches;

internal static class NetworkedPlayerInfoPatches
{
    [HarmonyPatch(typeof(NetworkedPlayerInfo), nameof(NetworkedPlayerInfo.SetTasks))]
    public static class DisableTasksPatch
    {
        public static bool Prefix(NetworkedPlayerInfo __instance)
        {
            if (GameManager.Instance == null ||
                !GameManager.Instance.TryGetComponent<CustomGameModeBehaviour>(out var customGameModeBehaviour) ||
                customGameModeBehaviour.CustomGameMode.Configuration.Tasks.Enabled) return true;

            __instance.Tasks.Clear();
            __instance.Object.SetTasks(__instance.Tasks);
            __instance.MarkDirty();

            return false;
        }
    }
}
