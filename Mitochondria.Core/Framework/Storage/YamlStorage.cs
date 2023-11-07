using Mitochondria.Core.Api.Storage;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Mitochondria.Core.Framework.Storage;

public class YamlStorage : IStorage
{
    public YamlStorageConfiguration StorageConfiguration { get; }

    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    private readonly Dictionary<string, object> _cache;

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

        _cache = new Dictionary<string, object>();
    }

    public void Save(string fileName, object obj)
    {
        var savePath = StorageConfiguration.GetAbsoluteSavePath(fileName);
        _cache[savePath] = obj;
        
        Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

        using var streamWriter = File.CreateText(savePath);
        _serializer.Serialize(streamWriter, obj);
    }

    public IStorageHandle<T> Load<T>(string fileName, IEnumerable<string>? altFileNames = null, bool useCached = true)
        where T : class
    {
        if (useCached &&
            _cache.TryGetValue(
                StorageConfiguration.GetAbsoluteSavePath(fileName),
                out var cachedObj) &&
            cachedObj is T typedObj)
        {
            return new StorageHandle<T>(typedObj, () => Save(fileName, typedObj));
        }
        
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

                return new StorageHandle<T>(obj, () => Save(fileName, obj));
            }
            catch
            {
                // ignored
            }
        }

        var newObj = (T) Activator.CreateInstance(typeof(T), true)!;

        Save(fileName, newObj);

        return new StorageHandle<T>(newObj, () => Save(fileName, newObj));
    }

    public void Delete(string fileName, IEnumerable<string>? altFileNames = null)
    {
        _cache.Remove(StorageConfiguration.GetAbsoluteSavePath(fileName));

        foreach (var path in StorageConfiguration.GetAbsoluteLoadPaths(fileName, altFileNames))
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}