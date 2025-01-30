namespace Mitochondria.Resources;

public class ResourceNode<T> : ResourceNode
{
    public override object? BoxedResource => Resource;

    public override Type ResourceType { get; } = typeof(T);

    public T? Resource { get; private set; }

    private readonly Action<ResourceNode<T>> _unload;
    private readonly List<ResourceHandle<T>> _handles;

    public ResourceNode(T resource, Action<ResourceNode<T>> unload)
    {
        Resource = resource;
        _unload = unload;

        _handles = new List<ResourceHandle<T>>();
    }

    public override ResourceHandle AcquireBoxedHandle()
        => AcquireHandle();

    public ResourceHandle<T> AcquireHandle()
    {
        var handle = new ResourceHandle<T>(this, ReleaseHandle);
        _handles.Add(handle);

        Loaded = true;

        return handle;
    }

    private void ReleaseHandle(ResourceHandle<T> handle)
    {
        _handles.Remove(handle);
        if (_handles.Count == 0)
        {
            _unload.Invoke(this);
            Resource = default;
            Loaded = false;
        }
    }
}

public abstract class ResourceNode
{
    public abstract object? BoxedResource { get; }

    public abstract Type ResourceType { get; }

    public bool Loaded { get; protected set; }

    public abstract ResourceHandle AcquireBoxedHandle();
}
