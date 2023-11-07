using BepInEx;

namespace Mitochondria.Core.Framework.Configuration.Extensions;

public static class ConfigExtensions
{
    public static PluginConfig GetConfig(this PluginInfo pluginInfo)
        => Config.Instance.Of(pluginInfo.Instance.GetType());
}