namespace Mitochondria.Api.Storage;

public interface IStorage
{
    public void Save(string fileName, object obj);

    public T Load<T>(string fileName, IEnumerable<string>? altFileNames = null, bool useCached = true)
        where T : class;

    public void Delete(string fileName, IEnumerable<string>? altFileNames = null);
}