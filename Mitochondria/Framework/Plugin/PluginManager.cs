using System.Collections.Immutable;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using Mitochondria.Framework.Utilities;

namespace Mitochondria.Framework.Plugin;

public static class PluginManager<TPlugin>
    where TPlugin : BasePlugin
{
    public static PluginInfo PluginInfo => _pluginInfo ??= PluginManager.Instance.GetPluginInfo<TPlugin>()!;

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

    public PluginInfo? GetPluginInfo<T>()
        where T : BasePlugin
    {
        return GetPluginInfo(typeof(T));
    }

    public PluginInfo? GetPluginInfo(Type pluginType)
        => _pluginInfos.TryGetValue(pluginType, out var pluginInfo) ? pluginInfo : null;
    

    internal void Add(Type pluginType, PluginInfo pluginInfo)
        => _pluginInfos.Add(pluginType, pluginInfo);
}