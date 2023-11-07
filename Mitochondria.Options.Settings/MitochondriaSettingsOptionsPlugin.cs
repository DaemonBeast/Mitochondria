using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mitochondria.Core;
using Reactor;

namespace Mitochondria.Options.Settings;

[BepInAutoPlugin("astral.mitochondria.options.settings")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MitochondriaPlugin.Id)]
public partial class MitochondriaSettingsOptionsPlugin : BasePlugin
{
    public Harmony Harmony { get; }

    public MitochondriaSettingsOptionsPlugin()
    {
        Harmony = new Harmony(Id);
    }

    public override void Load()
    {
        Harmony.PatchAll();
    }

    public override bool Unload()
    {
        Harmony.UnpatchSelf();

        return base.Unload();
    }
}