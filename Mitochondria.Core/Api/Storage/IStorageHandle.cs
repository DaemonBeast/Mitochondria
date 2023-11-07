namespace Mitochondria.Core.Api.Storage;

public interface IStorageHandle<out T> : IStorageHandle
{
    public T Obj { get; }
}

public interface IStorageHandle : IDisposable
{
    public void Save();
}