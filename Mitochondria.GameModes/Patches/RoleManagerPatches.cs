using HarmonyLib;

namespace Mitochondria.GameModes.Patches;

internal static class RoleManagerPatches
{
    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SetRole))]
    public static class DisableRoleSelectionPatch
    {
        public static bool Prefix()
            => GameManager.Instance == null ||
               !GameManager.Instance.TryGetComponent<CustomGameModeBehaviour>(out var customGameModeBehaviour) ||
               customGameModeBehaviour.CustomGameMode.Configuration.Roles.Enabled;
    }
}
