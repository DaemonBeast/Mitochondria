using HarmonyLib;

namespace Mitochondria.GameModes.Patches;

internal static class LogicGameFlowNormalPatches
{
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    public static class CheckEndCriteriaPatch
    {
        public static bool Prefix(LogicGameFlowNormal __instance)
        {
            if (!__instance.Manager.TryGetComponent<CustomGameModeBehaviour>(
                    out var customGameModeBehaviour)) return true;

            if (customGameModeBehaviour.CustomGameMode.Flow.ShouldEndGame(out var gameOverReason))
            {
                customGameModeBehaviour.CustomGameMode.Flow.BeforeGameEnd();

                __instance.Manager.RpcEndGame(gameOverReason.Value, false);
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
