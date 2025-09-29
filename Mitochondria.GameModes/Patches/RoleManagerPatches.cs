using HarmonyLib;
using Mitochondria.GameModes.Utilities;

namespace Mitochondria.GameModes.Patches;

internal static class RoleManagerPatches
{
    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SetRole))]
    public static class DisableRoleSelectionPatch
    {
        public static bool Prefix()
            => !CustomGameModeUtilities.TryGetActiveGameMode(out var customGameMode) ||
               customGameMode.Configuration.Roles.Enabled;
    }
}
