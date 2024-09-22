using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mitochondria.Core;

namespace Mitochondria.Resources;

[BepInAutoPlugin("astral.mitochondria.resources")]
[BepInProcess("Among Us.exe")]
[BepInDependency(MitochondriaCorePlugin.Id)]
public partial class MitochondriaResourcesPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

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
