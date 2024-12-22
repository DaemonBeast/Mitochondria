using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mitochondria.Core;

namespace Mitochondria.Input;

[BepInAutoPlugin("astral.mitochondria.input")]
[BepInProcess("Among Us.exe")]
[BepInDependency(MitochondriaCorePlugin.Id)]
public partial class MitochondriaInputPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        Keyboard.Initialize(this);

        Harmony.PatchAll();
    }

    public override bool Unload()
    {
        Harmony.UnpatchSelf();

        return base.Unload();
    }
}
