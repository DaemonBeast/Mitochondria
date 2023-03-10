using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mitochondria.Framework.Options;
using Mitochondria.Framework.Options.SettingsOptions;
using Mitochondria.Framework.Options.SettingsOptions.Converters;
using Mitochondria.Framework.Options.SettingsOptions.Factories;
using Mitochondria.Framework.Options.SettingsOptions.Providers;
using Mitochondria.Framework.Options.SettingsOptions.Providers.Handler;
using Mitochondria.Framework.Plugin;
using Mitochondria.Framework.Services;
using Mitochondria.Framework.Services.Extensions;
using Mitochondria.Framework.Utilities.Extensions;
using Mitochondria.Patches.Services;
using Reactor;
using Reactor.Utilities.Extensions;

namespace Mitochondria;

[BepInAutoPlugin("au.astral.mitochondria")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class MitochondriaPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    [CustomSettingsOption(order: 11)]
    [CustomSettingsOption(GameModes.HideNSeek, 11)]
    public static CustomToggleOption<MitochondriaPlugin> ShowTooltipsOption { get; private set; } = null!;

    private EventHooks? _eventHooks;

    public MitochondriaPlugin()
    {
        IL2CPPChainloader.Instance.PluginLoad += (pluginInfo, assembly, basePlugin) =>
        {
            PluginManager.Instance.Add(basePlugin.GetType(), pluginInfo);
            
            ServiceAttribute.Register(assembly, pluginInfo);
            CustomSettingsOptionConverterAttribute.Register(assembly);
            SettingsOptionProviderAttribute.Register(assembly);
            SettingsOptionFactoryAttribute.Register(assembly);
            SettingsOptionHandlerProviderAttribute.Register(assembly);
        };

        IL2CPPChainloader.Instance.PluginLoaded += pluginInfo =>
        {
            foreach (var service in pluginInfo.GetServices())
            {
                service.OnPluginLoaded(pluginInfo);
            }
        };
    }

    public override void Load()
    {
        _eventHooks = AddComponent<EventHooks>();
        
        Harmony.PatchAll();
        
        ShowTooltipsOption = new CustomToggleOption<MitochondriaPlugin>("Show Tooltips", true, false);
    }

    public override bool Unload()
    {
        _eventHooks.ThenIfNotNull(eventHooks => eventHooks.Destroy());
        
        Harmony.UnpatchSelf();

        return base.Unload();
    }
}