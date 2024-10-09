using System.Diagnostics;
using Mitochondria.Core.Utilities.Extensions;

namespace Mitochondria.Resources.FFmpeg.Utilities;

public static class FFmpegUtils
{
    public static async Task<Dictionary<string, string>> GetAudioFileInfoAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var process = CreateFFprobeProcess(
            $"-i \"{fileName}\" -show_format -show_streams -select_streams a:0 -of compact=p=0 -v 0");

        process.Start();

        var data = await process.StandardOutput.BaseStream.ReadFullyAsync(cancellationToken);
        var output = process.StandardOutput.CurrentEncoding.GetString(data);
        return ParseInfo(output);
    }

    public static async Task<Dictionary<string, string>> GetAudioFileInfoAsync(
        Stream inputStream,
        CancellationToken cancellationToken = default)
    {
        var process = CreateFFprobeProcess("-i - -show_format -show_streams -select_streams a:0 -of compact=p=0 -v 0");
        process.Start();

        var data = await FFmpegProcessUtils.PipeFullyAsync(process, inputStream, cancellationToken);
        var output = process.StandardOutput.CurrentEncoding.GetString(data);
        return ParseInfo(output);
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

    private static Dictionary<string, string> ParseInfo(string data)
    {
        return data
            .Split('|')
            .Select(p => p.Split('='))
            .DistinctBy(p => p[0])
            .ToDictionary(p => p[0], p => p[1]);
    }

    private static Process CreateProcess(string arguments)
    {
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
    }
}
