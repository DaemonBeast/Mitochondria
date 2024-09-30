using Mitochondria.Resources.FFmpeg.Utilities;
using Reactor.Utilities.Extensions;

namespace Mitochondria.Resources.FFmpeg;

public static class AudioConverter
{
    public static float[] ToPcmFloat32BitLittleEndian(
        string fileName,
        out int channels,
        out int sampleRate,
        out string? title)
    {
        var process = FFmpegUtils.CreateFFmpegProcess($"-i \"{fileName}\" -f f32le -acodec pcm_f32le pipe:1");
        process.Start();

        var data = process.StandardOutput.BaseStream.ReadFully();
        var samples = new float[data.Length / sizeof(float)];
        Buffer.BlockCopy(data, 0, samples, 0, data.Length);

        var info = FFmpegUtils.GetAudioFileInfo(fileName);
        channels = int.Parse(info["channels"]);
        sampleRate = int.Parse(info["sample_rate"]);
        title = info.GetValueOrDefault("tag:title"); 

        return samples;
    }
}
