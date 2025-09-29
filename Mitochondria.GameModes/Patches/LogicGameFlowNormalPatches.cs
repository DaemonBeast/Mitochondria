using HarmonyLib;
using Mitochondria.GameModes.Utilities.Extensions;

namespace Mitochondria.GameModes.Patches;

internal static class LogicGameFlowNormalPatches
{
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    public static class CheckEndCriteriaPatch
    {
        public static bool Prefix(LogicGameFlowNormal __instance)
        {
            if (!__instance.Manager.TryGetActiveCustomGameModeFlow(out var customGameModeFlow)) return true;

            if (customGameModeFlow.ShouldEndGame() is { ShouldEndGame: true, Reason: var reason })
            {
                customGameModeFlow.BeforeGameEnd();

                __instance.Manager.RpcEndGame(reason ?? GameOverReason.ImpostorsByKill, false);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    public static class IsGameOverDueToDeathPatch
    {
        public static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}
