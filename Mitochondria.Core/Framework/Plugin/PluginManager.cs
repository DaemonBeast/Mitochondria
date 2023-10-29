using System.Collections.Immutable;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using Mitochondria.Core.Framework.Utilities;

namespace Mitochondria.Core.Framework.Plugin;

public static class PluginManager<TPlugin>
    where TPlugin : BasePlugin
{
    public static PluginInfo PluginInfo => _pluginInfo ??= PluginManager.Instance.GetPluginInfo<TPlugin>();

    public static string PluginId => PluginInfo.Metadata.GUID;

    // ReSharper disable once StaticMemberInGenericType
    private static PluginInfo? _pluginInfo;
}

public class PluginManager
{
    public static PluginManager Instance => Singleton<PluginManager>.Instance;

    public IReadOnlyDictionary<Type, PluginInfo> PluginInfos => _pluginInfos.ToImmutableDictionary();

    private readonly Dictionary<Type, PluginInfo> _pluginInfos;

    private PluginManager()
    {
        _pluginInfos = new Dictionary<Type, PluginInfo>();
    }

    public PluginInfo GetPluginInfo<T>()
        where T : BasePlugin
    {
        return GetPluginInfo(typeof(T));
    }

    public PluginInfo GetPluginInfo(Type pluginType)
    {
        if (_pluginInfos.TryGetValue(pluginType, out var pluginInfo))
        {
            return pluginInfo;
        }

        if (!typeof(BasePlugin).IsAssignableFrom(pluginType))
        {
            throw new ArgumentException(
                $"Cannot get plugin info of {pluginType} as it does not extend {nameof(BasePlugin)}");
        }

        pluginInfo = IL2CPPChainloader.Instance.Plugins.Values
            .First(info => info.TypeName == pluginType.FullName);

        _pluginInfos[pluginType] = pluginInfo ??
                                   throw new ArgumentException(
                                       "Cannot get plugin info of plugin not loaded by BepInEx");

        return pluginInfo;
    }

    internal void Set(Type pluginType, PluginInfo pluginInfo)
        => _pluginInfos[pluginType] = pluginInfo;
}