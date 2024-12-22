using Mitochondria.Core.Utilities;
using Mitochondria.Core.Utilities.Extensions;
using Mitochondria.Resources.FFmpeg.Utilities;

namespace Mitochondria.Resources.FFmpeg;

public static class AudioConverter
{
    private const string PcmFloat32BitLittleEndianArguments = "-f f32le -acodec pcm_f32le";

    public static async Task<float[]> ToPcmF32LeAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var sanitizedFileName = FFmpegProcessUtils.VeryPoorlySanitizeFileName(fileName);

        using var process =
            FFmpegUtils.CreateFFmpegProcess($"-i \"{sanitizedFileName}\" {PcmFloat32BitLittleEndianArguments} pipe:1");

        process.Start();
        ChildProcessWatchdog.KillOnParentExit(process);

        var data = await process.StandardOutput.BaseStream.ReadFullyAsync(cancellationToken);
        return ToPcmF32LeSamples(data);
    }

    public static async Task<float[]> ToPcmF32LeAsync(
        Stream inputStream,
        CancellationToken cancellationToken = default)
    {
        using var process = FFmpegUtils.CreateFFmpegProcess($"-i - {PcmFloat32BitLittleEndianArguments} pipe:1");
        process.Start();
        ChildProcessWatchdog.KillOnParentExit(process);

        var data = await FFmpegProcessUtils.PipeFullyAsync(process, inputStream, cancellationToken);
        return ToPcmF32LeSamples(data);
    }

    public static IFFmpegStreamOwner ToPcmF32LeStream(string fileName)
    {
        var sanitizedFileName = FFmpegProcessUtils.VeryPoorlySanitizeFileName(fileName);

        var process =
            FFmpegUtils.CreateFFmpegProcess($"-i \"{sanitizedFileName}\" {PcmFloat32BitLittleEndianArguments} pipe:1");

        process.Start();
        ChildProcessWatchdog.KillOnParentExit(process);

        return new FFmpegOutputStreamOwner(process);
    }

    public static Stream ToPcmF32LeStream(
        Stream inputStream,
        CancellationToken cancellationToken = default)
    {
        var process = FFmpegUtils.CreateFFmpegProcess($"-i - {PcmFloat32BitLittleEndianArguments} pipe:1");
        process.Start();
        ChildProcessWatchdog.KillOnParentExit(process);

        _ = Task.Run(
            async () =>
            {
                await FFmpegProcessUtils.CopyToStandardInputAsync(process, inputStream, cancellationToken);
                // Prioritise closing the stdin stream so that FFmpeg knows we're done
                process.StandardInput.Close();
                process.Dispose();
            },
            cancellationToken);

        return process.StandardOutput.BaseStream;
    }

    public static float[] ToPcmF32LeSamples(byte[] data)
    {
        // TODO: handle endianness?

        var samples = new float[data.Length / sizeof(float)];
        Buffer.BlockCopy(data, 0, samples, 0, data.Length);

        return samples;
    }
}
