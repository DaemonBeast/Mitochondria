using HarmonyLib;
using Mitochondria.Core.Api.Services;
using Mitochondria.Core.Framework.Services;

namespace Mitochondria.Core.Patches;

[Service]
public class ModdedService : IService
{
    public void OnStart()
    {
        Constants.CompatVersions = Constants.CompatVersions.AddItem(ModdedPatches.ModdedBroadcastVersion).ToArray();
    }
}