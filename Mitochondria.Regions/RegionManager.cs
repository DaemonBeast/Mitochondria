namespace Mitochondria.Regions;

public static class RegionManager
{
    public static IEnumerable<CustomRegion> Regions => RegionsToAdd.ExceptBy(RegionsToRemove, region => region.Name);

    internal static readonly List<string> RegionsToRemove = new();

    private static readonly List<CustomRegion> RegionsToAdd = new();

    public static void Add(string name, Uri uri, ushort matchmakerPort = 443)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The name cannot be empty", nameof(name));
        }

        if (uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new ArgumentException("Uri scheme must be https", nameof(uri));
        }

        if (RegionsToRemove.Contains(name)) return;

        RegionsToAdd.Add(new CustomRegion(name, uri, matchmakerPort));
    }

    public static void ForceRemove(string regionName)
    {
        var customRegionIndex = RegionsToAdd.FindIndex(region => region.Name == regionName);
        if (customRegionIndex != -1) RegionsToAdd.RemoveAt(customRegionIndex);

        RegionsToRemove.Add(regionName);
    }

    public record CustomRegion(string Name, Uri Uri, ushort MatchmakerPort = 443);
}
