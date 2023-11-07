namespace Mitochondria.Core.Api.Storage;

public interface IStorage
{
    public void Save(string fileName, object obj);

    public IStorageHandle<T> Load<T>(string fileName, IEnumerable<string>? altFileNames = null, bool useCached = true)
        where T : class;

    public void Delete(string fileName, IEnumerable<string>? altFileNames = null);
}