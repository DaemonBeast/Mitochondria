using System.Reflection;
using Mitochondria.Resources.Provider;

namespace Mitochondria.Resources.Utilities;

public class EmbeddedResourceProvider<T> : ResourceProvider<T>
{
    public EmbeddedResourceProvider(Func<T> load, Action<ResourceNode<T>> unload) : base(load, unload)
    {
    }

    public record LoadInfo(string ResourceName, Func<Stream, T> LoadFromStream, Assembly? Assembly = null);
}
