using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mitochondria.Core;

namespace Mitochondria.Resources.Primitives;

[BepInAutoPlugin("astral.mitochondria.resources.primitives")]
[BepInProcess("Among Us.exe")]
[BepInDependency(MitochondriaCorePlugin.Id)]
public partial class MitochondriaResourcesPrimitivesPlugin : BasePlugin
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
