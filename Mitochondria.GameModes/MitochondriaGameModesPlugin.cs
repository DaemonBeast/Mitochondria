global using static Reactor.Utilities.Logger<Mitochondria.GameModes.MitochondriaGameModesPlugin>;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;

namespace Mitochondria.GameModes;

[BepInAutoPlugin("astral.mitochondria.gamemodes")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class MitochondriaGameModesPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        Harmony.PatchAll();

        IL2CPPChainloader.Instance.PluginLoaded += plugin =>
        {
            CustomGameModeAttribute.Register(plugin.Instance.GetType().Assembly);
        };
    }

    public override bool Unload()
    {
        Harmony.UnpatchSelf();

        return base.Unload();
    }
}

// TODO: Start button hover effect missing
