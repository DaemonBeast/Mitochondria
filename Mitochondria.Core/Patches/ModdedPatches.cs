using HarmonyLib;

namespace Mitochondria.Core.Patches;

internal static class ModdedPatches
{
    public static int OriginalBroadcastVersion => _originalBroadcastVersion ??= GetBroadcastVersionOriginal.Invoke();

    public static int ModdedBroadcastVersion =>
        _moddedBroadcastVersion ??= OriginalBroadcastVersion + Constants.MODDED_REVISION_MODIFIER_VALUE;

    private static int? _originalBroadcastVersion;
    private static int? _moddedBroadcastVersion;

    [HarmonyPatch(typeof(Constants), nameof(Constants.GetBroadcastVersion))]
    public static class GetBroadcastVersionOriginal
    {
        [HarmonyReversePatch]
        public static int Invoke()
        {
            throw new NotImplementedException("Stub was not replaced");
        }
    }

    [HarmonyPatch(typeof(Constants), nameof(Constants.GetBroadcastVersion))]
    public static class ModdedVersionGetPatch
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(ref int __result)
        {
            __result = ModdedBroadcastVersion;
        }
    }

    [HarmonyPatch(typeof(Constants), nameof(Constants.IsVersionModded))]
    public static class IsModdedPatch
    {
        public static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }
}