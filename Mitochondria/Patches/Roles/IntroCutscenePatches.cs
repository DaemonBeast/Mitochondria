using HarmonyLib;
using Mitochondria.Framework.Overrides;
using UnityEngine;

namespace Mitochondria.Patches.Roles;

internal static class IntroCutscenePatches
{
    private static readonly int ColorShaderId = Shader.PropertyToID("_Color");
    
    [HarmonyPatch]
    public static class TeamOverridePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        public static void Postfix(IntroCutscene __instance)
        {
            if (!OverrideManager.Instance.TryGetMergedOverride<IntroCutsceneTeamOverride>(out var teamOverride))
            {
                return;
            }

            if (teamOverride.Title != null)
            {
                __instance.TeamTitle.text = teamOverride.Title;
            }

            if (teamOverride.Color != null)
            {
                __instance.BackgroundBar.material.SetColor(ColorShaderId, teamOverride.Color.Value);
                __instance.TeamTitle.color = teamOverride.Color.Value;
            }
        }
    }
    
    [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.TeamColor), MethodType.Getter)]
    public static class RoleColorOverridePatch
    {
        public static void Postfix(ref Color __result)
        {
            if (!OverrideManager.Instance.TryGetMergedOverride<IntroCutsceneRoleColorOverride>(
                    out var roleColorOverride))
            {
                return;
            }

            var overrideColor = roleColorOverride.Color;
            if (overrideColor == null)
            {
                return;
            }

            __result = overrideColor.Value;
        }
    }
}