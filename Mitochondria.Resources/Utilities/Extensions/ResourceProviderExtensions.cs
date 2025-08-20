using Mitochondria.Resources.Provider;

namespace Mitochondria.Resources.Utilities.Extensions;

public static class ResourceProviderExtensions
{
    public static ResourceProvider<T> AsResourceProvider<T>(this T resource)
        => new(() => resource, _ => { });
}
