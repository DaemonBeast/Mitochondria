namespace Mitochondria.Api.Storage;

public interface IStorage
{
    public void Save(string fileName, object obj);

    public T Load<T>(string fileName, IEnumerable<string>? altFileNames = null)
        where T : class;
}