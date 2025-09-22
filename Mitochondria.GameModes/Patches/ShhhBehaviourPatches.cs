using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;

namespace Mitochondria.GameModes.Patches;

internal static class ShhhBehaviourPatches
{
    [HarmonyPatch(typeof(ShhhBehaviour), nameof(ShhhBehaviour.PlayAnimation))]
    public static class ShowEmblemPatch
    {
        public static bool Prefix(ShhhBehaviour __instance, ref Il2CppSystem.Collections.IEnumerator __result)
        {
            if (!GameManager.Instance.TryGetComponent<CustomGameModeBehaviour>(
                    out var customGameModeBehaviour) ||
                customGameModeBehaviour.CustomGameMode.Configuration.IntroCutscene.ShowEmblem) return true;

            __instance.gameObject.SetActive(false);
            __result = CoEmpty().WrapToIl2Cpp();
            return false;

            IEnumerator CoEmpty()
            {
                yield break;
            }
        }
    }
}
