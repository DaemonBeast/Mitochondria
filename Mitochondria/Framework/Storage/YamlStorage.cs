using Mitochondria.Api.Storage;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Mitochondria.Framework.Storage;

public class YamlStorage : IStorage
{
    public YamlStorageConfiguration StorageConfiguration { get; }

    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public YamlStorage(YamlStorageConfiguration storageConfiguration)
    {
        StorageConfiguration = storageConfiguration;

        _serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        _deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();
    }

    public void Save(string fileName, object obj)
    {
        var savePath = StorageConfiguration.GetAbsoluteSavePath(fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

        using var streamWriter = File.CreateText(savePath);
        _serializer.Serialize(streamWriter, obj);
    }

    public T Load<T>(string fileName, IEnumerable<string>? altFileNames = null)
        where T : class
    {
        foreach (var path in StorageConfiguration.GetAbsoluteLoadPaths(fileName, altFileNames))
        {
            if (!File.Exists(path))
            {
                continue;
            }

            try
            {
                T? obj;

                using (var streamReader = File.OpenText(path))
                {
                    obj = _deserializer.Deserialize<T?>(streamReader);
                }

                if (obj == null)
                {
                    continue;
                }

                Save(fileName, obj);

                return obj;
            }
            catch
            {
                // ignored
            }
        }

        var newObj = (T) Activator.CreateInstance(typeof(T), true)!;
        Save(fileName, newObj);

        return newObj;
    }
}