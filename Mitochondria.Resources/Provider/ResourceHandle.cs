namespace Mitochondria.Resources.Provider;

public class ResourceHandle<T> : ResourceHandle
{
    public override object BoxedResource => _node.Resource!;

    public override Type ResourceType { get; } = typeof(T);

    public override bool Disposed { get; protected set; }

    public T Resource => _node.Resource!;

    private readonly ResourceNode<T> _node;
    private readonly Action<ResourceHandle<T>> _release;

    internal ResourceHandle(ResourceNode<T> node, Action<ResourceHandle<T>> release)
    {
        _node = node;
        _release = release;
    }

    public override void Dispose()
    {
        if (Disposed)
        {
            return;
        }

        Disposed = true;

        _release.Invoke(this);

        GC.SuppressFinalize(this);
    }
}

public abstract class ResourceHandle : IDisposable
{
    public abstract object BoxedResource { get; }

    public abstract Type ResourceType { get; }

    public abstract bool Disposed { get; protected set; }

    protected ResourceHandle()
    {
    }

    public abstract void Dispose();
}
