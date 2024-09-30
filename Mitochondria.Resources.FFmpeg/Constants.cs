namespace Mitochondria.Resources.FFmpeg;

public static class Constants
{
    public static class Paths
    {
        // Make sure they exist on plugin load
        public static readonly string FFmpegExe =
            Directory.GetFiles(BepInEx.Paths.PluginPath, "ffmpeg.exe", SearchOption.AllDirectories).SingleOrDefault()!;

        public static readonly string FFprobeExe =
            Directory.GetFiles(BepInEx.Paths.PluginPath, "ffprobe.exe", SearchOption.AllDirectories).SingleOrDefault()!;
    }
}
