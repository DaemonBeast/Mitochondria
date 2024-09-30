using System.Diagnostics;
using Reactor.Utilities.Extensions;

namespace Mitochondria.Resources.FFmpeg.Utilities;

public static class FFmpegUtils
{
    public static Dictionary<string, string> GetAudioFileInfo(string fileName)
    {
        var process = CreateFFprobeProcess(
            $"-i \"{fileName}\" -show_format -show_streams -select_streams a:0 -of compact=p=0 -v 0");

        process.Start();

        var data = process.StandardOutput.BaseStream.ReadFully();
        var output = process.StandardOutput.CurrentEncoding.GetString(data);

        return output.Split('|').Select(p => p.Split('=')).ToDictionary(p => p[0], p => p[1]);
    }

    public static Process CreateFFmpegProcess(string arguments)
    {
        var process = CreateProcess(arguments);
        process.StartInfo.FileName = Constants.Paths.FFmpegExe;

        return process;
    }

    public static Process CreateFFprobeProcess(string arguments)
    {
        var process = CreateProcess(arguments);
        process.StartInfo.FileName = Constants.Paths.FFprobeExe;

        return process;
    }

    private static Process CreateProcess(string arguments)
    {
        var process = new Process();
        var startInfo = process.StartInfo;
        startInfo.Arguments = arguments;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardInput = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        return process;
    }
}
