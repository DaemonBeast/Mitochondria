using AmongUs.GameOptions;
using HarmonyLib;
using Mitochondria.Framework.Roles;
using Mitochondria.Framework.Utilities.Extensions;

namespace Mitochondria.Patches.Roles;

internal static class RolePatches
{
    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SetRole))]
    public static class OtherRolePatch
    {
        public static void Prefix(PlayerControl targetPlayer, RoleTypes roleType)
        {
            if (targetPlayer == null)
            {
                return;
            }

            var playerInfo = targetPlayer.Data;
            if (playerInfo == null)
            {
                return;
            }
            
            var customRoleBehaviour = targetPlayer.GetCustomRoleBehaviour();
            customRoleBehaviour.ClearRole();

            if (CustomRoleManager.Instance.TryGet(roleType, out var customRole))
            {
                customRoleBehaviour.SetRole(customRole.GetType());
            }
        }
    }
    
    [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.IsImpostor), MethodType.Getter)]
    public static class IsImpostorPatch
    {
        public static bool Prefix(RoleBehaviour __instance, ref bool __result)
        {
            __result = IsImpostor(__instance);

            return false;
        }
    }

    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.IsImpostorRole))]
    public static class IsImpostorRolePatch
    {
        public static bool Prefix(RoleManager __instance, RoleTypes roleType, ref bool __result)
        {
            var roleBehaviour = __instance.AllRoles.FirstOrDefault(r => r.Role == roleType);

            __result = roleBehaviour != null && IsImpostor(roleBehaviour);
            return false;
        }
    }

    // TODO: Figure out how to patch `RoleBehaviour.AppendTaskHint` without issues instead,
    // as this one patches after the hide and seek text is added, which varies from base game behaviour
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class TaskHintPatch
    {
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;

            if (__instance.taskDirtyTimer != 0f ||
                player == null ||
                player.Data == null ||
                player.myTasks == null ||
                player.myTasks.Count == 0)
            {
                return;
            }

            var role = player.Data.Role;
            
            if (role == null ||
                !player.TryGetCustomRoleBehaviour(out var customRoleBehaviour) ||
                customRoleBehaviour.CustomRole == null ||
                string.IsNullOrEmpty(role.BlurbMed))
            {
                return;
            }
            
            __instance.tasksString.AppendFormat(
                "\n\n{0} {1}\n{2}",
                role.NiceName,
                StringNames.RoleHint.GetTranslation(),
                role.BlurbMed);
            
            __instance.tasksString.TrimEnd();
            __instance.TaskPanel.SetTaskText(__instance.tasksString.ToString());
        }
    }

    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
    public static class FixInlinedSabotageButtonImpostorCheckPatch
    {
        public static bool Prefix()
        {
            var player = PlayerControl.LocalPlayer;
            if (IsImpostor(player.Data.Role) && !player.inVent && GameManager.Instance.SabotagesEnabled())
            {
                HudManager.Instance.ToggleMapVisible(new MapOptions
                {
                    Mode = MapOptions.Modes.Sabotage
                });
            }
            
            return false;
        }
    }

    private static bool IsImpostor(RoleBehaviour roleBehaviour)
    {
        var teamType = roleBehaviour.TeamType;

        return RoleSideManager.Instance.TryGetRoleSide(teamType, out var roleSide)
            ? roleSide.IsImpostor
            : teamType == RoleTeamTypes.Impostor;
    }
}