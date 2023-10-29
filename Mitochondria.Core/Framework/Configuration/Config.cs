using BepInEx.Unity.IL2CPP;
using Mitochondria.Core.Api.Configuration;
using Mitochondria.Core.Framework.Plugin;
using Mitochondria.Core.Framework.Storage;
using Mitochondria.Core.Framework.Utilities;

namespace Mitochondria.Core.Framework.Configuration;

public static class Config<TPlugin>
    where TPlugin : BasePlugin
{
    public static PluginConfig Instance => Config.Instance.Of(typeof(TPlugin));
}

public class Config
{
    public static Config Instance => Singleton<Config>.Instance;

    private readonly Dictionary<Type, PluginConfig> _configs;

    private Config()
    {
        _configs = new Dictionary<Type, PluginConfig>();
    }

    public PluginConfig Of(Type pluginType)
    {
        if (_configs.TryGetValue(pluginType, out var pluginConfig))
        {
            return pluginConfig;
        }

        pluginConfig = new PluginConfig(pluginType);
        _configs.Add(pluginType, pluginConfig);

        return pluginConfig;
    }
}

public class PluginConfig
{
    public YamlStorage Storage { get; }

    internal PluginConfig(Type pluginType)
    {
        Storage = new YamlStorage(
            new YamlStorageConfiguration(PluginManager.Instance.GetPluginInfo(pluginType).Metadata.GUID));
    }

    public void Save(IConfig config)
        => Storage.Save(config.ConfigName, config);

    public TConfig Load<TConfig>()
        where TConfig : class, IConfig
        => Storage.Load<TConfig>(ConfigNameLookup<TConfig>.ConfigName);
}

internal static class ConfigNameLookup<TConfig>
    where TConfig : class, IConfig
{
    public static string ConfigName =>
        _configName ??= ((IConfig) Activator.CreateInstance(typeof(TConfig), true)!).ConfigName;

    // ReSharper disable once StaticMemberInGenericType
    private static string? _configName;
}