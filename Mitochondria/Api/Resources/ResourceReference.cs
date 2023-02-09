namespace Mitochondria.Api.Resources;

public abstract class ResourceReference<T> : IResourceReference<T>
    where T : class
{
    public abstract T Load(bool useCached = true);

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

public interface IResourceReference<out T> : IResourceReference
    where T : class
{
    public T Load(bool useCached = true);
}

public interface IResourceReference
{
    public object LoadBoxed(bool useCached = true);

    public T? TryLoad<T>(bool useCached = true)
        where T : class;
}