using System.Text.Json;

namespace Mitochondria.Core.Utilities;

public static class JsonUtils
{
    public static async ValueTask<T> DeserializeAsyncOrNew<T>(string filePath, Func<T> factory)
    {
        T obj;

        if (File.Exists(filePath))
        {
            try
            {
                await using var objFileStream = File.OpenRead(filePath);
                obj = (await JsonSerializer.DeserializeAsync<T>(objFileStream))!;
            }
            catch
            {
                obj = factory.Invoke();
            }
        }
        else
        {
            obj = factory.Invoke();
        }

        return obj;
    }
}
