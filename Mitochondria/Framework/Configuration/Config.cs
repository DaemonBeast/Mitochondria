using BepInEx.Unity.IL2CPP;
using Mitochondria.Api.Configuration;
using Mitochondria.Framework.Plugin;
using Mitochondria.Framework.Storage;

namespace Mitochondria.Framework.Configuration;

public static class Config<TPlugin>
    where TPlugin : BasePlugin
{
    // ReSharper disable once StaticMemberInGenericType
    public static YamlStorage Storage { get; }

    static Config()
    {
        Storage = new YamlStorage(new YamlStorageConfiguration(PluginManager<TPlugin>.PluginId));
    }

    public static void Save(IConfig config)
        => Storage.Save(config.ConfigName, config);

    public static TConfig Load<TConfig>()
        where TConfig : class, IConfig
        => Storage.Load<TConfig>(ConfigNameLookup<TConfig>.ConfigName);
}

internal static class ConfigNameLookup<TConfig>
    where TConfig : class, IConfig
{
    public static string ConfigName =>
        _configName ??= ((TConfig) Activator.CreateInstance(typeof(TConfig), true)!).ConfigName;

    // ReSharper disable once StaticMemberInGenericType
    private static string? _configName;
}