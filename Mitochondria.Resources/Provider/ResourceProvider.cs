namespace Mitochondria.Resources.Provider;

public class ResourceProvider<T> : ResourceProvider
{
    public override Type ResourceType { get; } = typeof(T);

    private ResourceNode<T>? _node;

    private readonly Func<T> _load;
    private readonly Action<ResourceNode<T>> _unload;

    public ResourceProvider(Func<T> load, Action<ResourceNode<T>> unload)
    {
        _load = load;
        _unload = unload;
    }

    public override ResourceHandle AcquireBoxedHandle()
        => AcquireHandle();

    public ResourceHandle<T> AcquireHandle()
    {
        _node ??= new ResourceNode<T>(_load.Invoke(), UnloadNode);
        return _node.AcquireHandle();
    }

    private void UnloadNode(ResourceNode<T> node)
    {
        _unload.Invoke(node);
        _node = null;
    }
}

public abstract class ResourceProvider
{
    public abstract Type ResourceType { get; }

    public abstract ResourceHandle AcquireBoxedHandle();
}
