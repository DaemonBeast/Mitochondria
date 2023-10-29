using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mitochondria.Core.Framework.Binding;
using Mitochondria.Core.Framework.Plugin;
using Mitochondria.Core.Framework.Roles;
using Mitochondria.Core.Framework.Services;
using Mitochondria.Core.Framework.Services.Extensions;
using Mitochondria.Core.Framework.Utilities.Extensions;
using Mitochondria.Core.Patches.Services;
using Reactor;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;

namespace Mitochondria.Core;

[BepInAutoPlugin("astral.mitochondria.core")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class MitochondriaPlugin : BasePlugin
{
    public Harmony Harmony { get; }

    private EventHooks? _eventHooks;

    public MitochondriaPlugin()
    {
        Harmony = new Harmony(Id);

        IL2CPPChainloader.Instance.PluginLoad += (pluginInfo, assembly, basePlugin) =>
        {
            if (PluginManager.Instance.PluginInfos.Values
                .Select(p => p.Instance?.GetType()?.Assembly)
                .Any(a => a == assembly))
            {
                Logger<MitochondriaPlugin>.Error(
                    $"Multiple plugins in the same assembly is not supported. {pluginInfo.TypeName} may not work properly");

                return;
            }

            PluginManager.Instance.Set(basePlugin.GetType(), pluginInfo);
            
            ServiceAttribute.Register(assembly, pluginInfo);
            CustomRoleAttribute.Register(assembly);
            ConverterAttribute.Register(assembly);
            BindingAttribute.Register(assembly);
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
    }

    public override bool Unload()
    {
        _eventHooks.ThenIfNotNull(eventHooks => eventHooks.Destroy());
        
        Harmony.UnpatchSelf();

        return base.Unload();
    }
}