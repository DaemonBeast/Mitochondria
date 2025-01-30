using HarmonyLib;
using UnityEngine.AddressableAssets;

namespace Mitochondria.Resources.Addressables.Patches;

internal static class AssetReferencePatches
{
    public static class RuntimeKeyPatch
    {
        [HarmonyPatch(typeof(AssetReference), nameof(AssetReference.RuntimeKeyIsValid))]
        public static class A
        {
            public static bool Prefix(AssetReference __instance, ref bool __result)
            {
                __result = AssetReferencePatch.RuntimeKeyIsValidOriginal(__instance) ||
                           CustomAddressables.ResourceProviders.ContainsKey(__instance.AssetGUID);

                return false;
            }
        }

        [HarmonyPatch(typeof(AssetReference), nameof(AssetReference.RuntimeKeyIsValid))]
        public static class AssetReferencePatch
        {
            [HarmonyReversePatch]
            public static bool RuntimeKeyIsValidOriginal(AssetReference instance)
                => throw new NotSupportedException();
        }
    }
}
