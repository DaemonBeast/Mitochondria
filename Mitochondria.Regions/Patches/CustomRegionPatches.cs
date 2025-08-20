using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace Mitochondria.Regions.Patches;

internal static class CustomRegionPatches
{
    [HarmonyPatch(typeof(ServerManager), nameof(ServerManager.SaveServers))]
    public static class AddCustomRegionsPatch
    {
        public static void Prefix(ServerManager __instance)
        {
            var existingRegions = __instance.AvailableRegions
                .Where(existingRegion => !RegionManager.RegionsToRemove.Contains(existingRegion.Name));

            var regionsToAdd = RegionManager.Regions
                .Where(customRegion => existingRegions.All(existingRegion => existingRegion.Name != customRegion.Name))
                .Select(WrapToAmongUs);

            __instance.AvailableRegions = existingRegions.Concat(regionsToAdd).ToArray();
        }

        private static IRegionInfo WrapToAmongUs(RegionManager.CustomRegion region)
        {
            var uri = region.Uri.AbsoluteUri.TrimEnd('/').TrimEnd('\\');

            return new StaticHttpRegionInfo(
                region.Name,
                StringNames.NoTranslation,
                uri,
                new Il2CppReferenceArray<ServerInfo>(new[]
                {
                    new ServerInfo("Http-1", uri, region.MatchmakerPort, false)
                })).Cast<IRegionInfo>();
        }
    }
}
