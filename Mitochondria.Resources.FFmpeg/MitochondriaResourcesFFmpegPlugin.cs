using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mitochondria.Core;

namespace Mitochondria.Resources.FFmpeg;

[BepInAutoPlugin("astral.mitochondria.resources.ffmpeg")]
[BepInProcess("Among Us.exe")]
[BepInDependency(MitochondriaCorePlugin.Id)]
public partial class MitochondriaResourcesFFmpegPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        if (Core.Constants.Paths.FFmpegExe == null || Core.Constants.Paths.FFprobeExe == null)
        {
            throw new FileNotFoundException("FFmpeg or FFprobe executable not found");
        }

        Harmony.PatchAll();
    }

    public override bool Unload()
    {
        Harmony.UnpatchSelf();

        return base.Unload();
    }
}
