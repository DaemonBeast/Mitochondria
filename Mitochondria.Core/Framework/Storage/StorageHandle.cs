using Mitochondria.Core.Api.Storage;

namespace Mitochondria.Core.Framework.Storage;

public class StorageHandle<T> : IStorageHandle<T>
{
    public T Obj { get; }

    private readonly Action _saveHandler;

    public StorageHandle(T obj, Action saveHandler)
    {
        Obj = obj;
        _saveHandler = saveHandler;
    }

    public void Save()
        => _saveHandler.Invoke();

    public void Dispose()
        => Save();
}