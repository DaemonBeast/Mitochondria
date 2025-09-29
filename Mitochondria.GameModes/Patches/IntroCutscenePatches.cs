using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Mitochondria.GameModes.Utilities;
using Reactor.Utilities.Extensions;

namespace Mitochondria.GameModes.Patches;

internal static class IntroCutscenePatches
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CoBegin))]
    public static class IntroCutsceneSkipPatch
    {
        public static void Postfix(IntroCutscene __instance, ref Il2CppSystem.Collections.IEnumerator __result)
            => __result = CoPostfix(__result, __instance).WrapToIl2Cpp();

        private static IEnumerator CoPostfix(
            Il2CppSystem.Collections.IEnumerator originalEnumerator,
            IntroCutscene introCutscene)
        {
            if (!CustomGameModeUtilities.TryGetActiveGameMode(out var customGameMode) || customGameMode.Flow == null)
            {
                yield return originalEnumerator.WrapToManaged();
                yield break;
            }

            var coIntroCutscene = EnumeratorUtilities.ConcatAll(
                customGameMode.Configuration.IntroCutscene.IntroCutsceneTypes
                    .Select(type =>
                        ((BaseCustomGameModeIntroCutscene) Activator.CreateInstance(type)!).CoIntroCutscene())
                    .ToArray());

            var coBeforeCutsceneEnds = EnumeratorUtilities.WhenAll(
                GameManager.Instance.StartCoroutine,
                coIntroCutscene,
                customGameMode.Flow.CoSetupGame());

            if (customGameMode.Configuration.IntroCutscene.ShowTeamAndRole)
            {
                yield return originalEnumerator.WrapToManaged();
            }
            else
            {
                yield return ShipStatus.Instance.CosmeticsCache.PopulateFromPlayers();

                ShipStatus.Instance.StartSFX();
                introCutscene.gameObject.Destroy();
            }

            yield return coBeforeCutsceneEnds;
        }
    }
}
