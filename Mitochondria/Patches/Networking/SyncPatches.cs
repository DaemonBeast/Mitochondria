using HarmonyLib;
using Mitochondria.Framework.Networking;

namespace Mitochondria.Patches.Networking;

public static class SyncPatches
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