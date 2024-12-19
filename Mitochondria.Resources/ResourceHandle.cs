namespace Mitochondria.Resources;

public class ResourceHandle<T> : IDisposable
{
    public T Resource => _node.Resource!;

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
        public T? Resource { get; private set; }

        public bool Loaded { get; private set; }

        private readonly Action<ResourceNode> _unload;
        private readonly List<ResourceHandle<T>> _handles;

        public ResourceNode(T resource, Action<ResourceNode> unload)
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
                _unload.Invoke(this);
                Resource = default;
            }
        }
    }

    public class ResourceProvider
    {
        private ResourceNode? _node;

        private readonly Func<T> _load;
        private readonly Action<ResourceNode> _unload;

        public ResourceProvider(Func<T> load, Action<ResourceNode> unload)
        {
            _load = load;
            _unload = unload;
        }

        public ResourceHandle<T> AcquireHandle()
        {
            _node ??= new ResourceNode(_load.Invoke(), UnloadNode);
            return _node.AcquireHandle();
        }

        private void UnloadNode(ResourceNode node)
        {
            _unload.Invoke(node);
            _node = null;
        }
    }
}
