using Mitochondria.Core.Utilities.Extensions;
using Mitochondria.Resources.FFmpeg.Utilities;

namespace Mitochondria.Resources.FFmpeg;

public static class AudioConverter
{
    public static async Task<float[]> ToPcmFloat32BitLittleEndianAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var process = FFmpegUtils.CreateFFmpegProcess($"-i \"{fileName}\" -f f32le -acodec pcm_f32le pipe:1");
        process.Start();

        var data = await process.StandardOutput.BaseStream.ReadFullyAsync(cancellationToken);
        return ToSamples(data);
    }

    public static async Task<float[]> ToPcmFloat32BitLittleEndianAsync(
        Stream inputStream,
        CancellationToken cancellationToken = default)
    {
        var process = FFmpegUtils.CreateFFmpegProcess("-i - -f f32le -acodec pcm_f32le pipe:1");
        process.Start();

        var data = await FFmpegProcessUtils.PipeFullyAsync(process, inputStream, cancellationToken);
        return ToSamples(data);
    }

    private static float[] ToSamples(byte[] data)
    {
        var samples = new float[data.Length / sizeof(float)];
        Buffer.BlockCopy(data, 0, samples, 0, data.Length);

        return samples;
    }
}
