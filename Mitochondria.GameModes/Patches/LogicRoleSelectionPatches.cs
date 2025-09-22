using HarmonyLib;

namespace Mitochondria.GameModes.Patches;

internal static class LogicRoleSelectionPatches
{
    [HarmonyPatch(typeof(LogicRoleSelection), nameof(LogicRoleSelection.AssignRolesForTeam))]
    public static class DisableRoleSelectionPatch
    {
        public static bool Prefix()
            => GameManager.Instance == null ||
               !GameManager.Instance.TryGetComponent<CustomGameModeBehaviour>(out var customGameModeBehaviour) ||
               customGameModeBehaviour.CustomGameMode.Configuration.Roles.Enabled;
    }
}
