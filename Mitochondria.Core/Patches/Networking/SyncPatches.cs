using HarmonyLib;
using Mitochondria.Core.Framework.Networking;

namespace Mitochondria.Core.Patches.Networking;

internal static class SyncPatches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
    public static class SyncOnJoin
    {
        public static void Postfix()
        {
            SyncableManager.Instance.SyncAll();
        }
    }
}