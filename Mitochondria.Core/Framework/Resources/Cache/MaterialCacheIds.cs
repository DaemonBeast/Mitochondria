namespace Mitochondria.Core.Framework.Resources.Cache;

public static class MaterialCacheIds
{
    public static int RedAndWhiteText { get; }

    static MaterialCacheIds()
    {
        RedAndWhiteText = MaterialCache.Instance.Reserve();
    }
}