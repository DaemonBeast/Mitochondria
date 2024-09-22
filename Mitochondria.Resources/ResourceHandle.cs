namespace Mitochondria.Resources;

public class ResourceHandle<T> : IDisposable
{
    public T Resource => _node.Resource;

    public bool Loaded => _node.Loaded;

    public bool Disposed { get; private set; }

    private readonly ResourceNode _node;
    private readonly Action<ResourceHandle<T>> _release;

    private ResourceHandle(ResourceNode node, Action<ResourceHandle<T>> release)
    {
        _node = node;
        _release = release;
    }

    public void Dispose()
    {
        if (Disposed)
        {
            return;
        }

        Disposed = true;

        _release.Invoke(this);

        GC.SuppressFinalize(this);
    }

    public class ResourceNode
    {
        public T Resource { get; }

        public bool Loaded { get; private set; }

        private readonly Action _unload;
        private readonly List<ResourceHandle<T>> _handles;

        public ResourceNode(T resource, Action unload)
        {
            Resource = resource;
            _unload = unload;

            _handles = new List<ResourceHandle<T>>();
        }

        public ResourceHandle<T> AcquireHandle()
        {
            var handle = new ResourceHandle<T>(this, ReleaseHandle);
            _handles.Add(handle);

            return handle;
        }

        private void ReleaseHandle(ResourceHandle<T> handle)
        {
            _handles.Remove(handle);
            if (_handles.Count == 0)
            {
                _unload.Invoke();
            }
        }
    }
}
