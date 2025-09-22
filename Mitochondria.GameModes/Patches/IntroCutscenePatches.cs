using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
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
            if (GameManager.Instance.TryGetComponent<CustomGameModeBehaviour>(out var customGameModeBehaviour) &&
                !customGameModeBehaviour.CustomGameMode.Configuration.IntroCutscene.ShowTeamAndRole)
            {
                yield return ShipStatus.Instance.CosmeticsCache.PopulateFromPlayers();

                ShipStatus.Instance.StartSFX();
                introCutscene.gameObject.Destroy();

                yield return customGameModeBehaviour.CustomGameMode.Flow.CoBeforeGameStart();

                yield break;
            }

            while (originalEnumerator.MoveNext()) yield return originalEnumerator.Current;

            yield return customGameModeBehaviour.CustomGameMode.Flow.CoBeforeGameStart();
        }
    }
}
