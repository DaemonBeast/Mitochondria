using Mitochondria.Resources.Addressables;
using Mitochondria.Resources.Provider;

namespace Mitochondria.Resources.Utilities.Extensions;

public static class CustomAddressablesExtensions
{
    public static void RegisterAsAddressable(this ResourceProvider provider, string guid)
        => CustomAddressables.ResourceProviders.Add(guid, provider);
}
