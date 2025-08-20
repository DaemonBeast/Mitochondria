using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mitochondria.Utilities;
using Mitochondria.Resources.Addressables.Patches;
using Reactor;

namespace Mitochondria.Resources;

[BepInAutoPlugin("astral.mitochondria.resources")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MitochondriaUtilitiesPlugin.Id)]
public partial class MitochondriaResourcesPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        AddressablesPatches.LoadAssetPatch.Initialize();

        Harmony.PatchAll();
    }

    public override bool Unload()
    {
        Harmony.UnpatchSelf();

        return base.Unload();
    }
}
