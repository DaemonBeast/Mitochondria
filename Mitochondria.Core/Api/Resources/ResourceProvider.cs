using System.Collections;
using Reactor.Utilities;

namespace Mitochondria.Core.Api.Resources;

public abstract class ResourceProvider<T> : IResourceProvider<T>
    where T : class
{
    public abstract T Load(bool useCached = true);

    public virtual IEnumerator CoLoad(bool skipIfCached = true, Action<T>? onCompleted = null)
    {
        onCompleted?.Invoke(Load(skipIfCached));
        yield break;
    }

    public void Preload()
    {
        Coroutines.Start(CoLoad());
    }

    public object LoadBoxed(bool useCached = true)
    {
        return Load(useCached);
    }

    public T1? TryLoad<T1>(bool useCached = true)
        where T1 : class
    {
        return Load(useCached) as T1;
    }
}

public interface IResourceProvider<out T> : IResourceProvider
    where T : class
{
    public T Load(bool useCached = true);

    public IEnumerator CoLoad(bool skipIfCached = true, Action<T>? onCompleted = null);
}

public interface IResourceProvider
{
    public object LoadBoxed(bool useCached = true);

    public T? TryLoad<T>(bool useCached = true)
        where T : class;
}