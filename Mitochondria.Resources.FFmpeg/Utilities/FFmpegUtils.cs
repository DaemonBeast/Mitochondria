using System.Diagnostics;
using Mitochondria.Utilities;
using Mitochondria.Utilities.Extensions;

namespace Mitochondria.Resources.FFmpeg.Utilities;

public static class FFmpegUtils
{
    private const string AudioInfoArguments =
        "-show_packets -show_format -show_streams -read_intervals 999999999 -select_streams a:0 -of flat=h=0 -v 0";

    public static async Task<Dictionary<string, string>> GetAudioFileInfoAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var sanitizedFileName = FFmpegProcessUtils.VeryPoorlySanitizeFileName(fileName);

        using var process = CreateFFprobeProcess($"-i \"{sanitizedFileName}\" {AudioInfoArguments}");
        process.Start();
        ChildProcessWatchdog.KillOnParentExit(process);

        var data = await process.StandardOutput.BaseStream.ReadFullyAsync(cancellationToken);
        var output = process.StandardOutput.CurrentEncoding.GetString(data);
        return ParseInfo(output);
    }

    public static async Task<Dictionary<string, string>> GetAudioFileInfoAsync(
        Stream inputStream,
        CancellationToken cancellationToken = default)
    {
        using var process = CreateFFprobeProcess($"-i - {AudioInfoArguments}");
        process.Start();
        ChildProcessWatchdog.KillOnParentExit(process);

        var encoding = process.StandardOutput.CurrentEncoding;
        var data = await FFmpegProcessUtils.PipeFullyAsync(process, inputStream, cancellationToken);
        process.Close();

        var output = encoding.GetString(data);
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
            .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Split('=', 2))
            .Where(p => p[1] != "\"N/A\"")
            .ToDictionary(p => p[0], p => p[1].Trim('\"'));
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
                RedirectStandardOutput = true
            }
        };
    }
}
