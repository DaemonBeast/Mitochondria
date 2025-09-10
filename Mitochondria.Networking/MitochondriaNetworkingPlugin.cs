global using static Reactor.Utilities.Logger<Mitochondria.Networking.MitochondriaNetworkingPlugin>;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mitochondria.Networking.Utilities;
using Reactor;

namespace Mitochondria.Networking;

[BepInAutoPlugin("astral.mitochondria.networking")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class MitochondriaNetworkingPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        Harmony.PatchAll();

        AddComponent<SynchronizationUtilities.NtpSynchronizeBehaviour>();
    }

    public override bool Unload()
    {
        Harmony.UnpatchSelf();

        return base.Unload();
    }
}
