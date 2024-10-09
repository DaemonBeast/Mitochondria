using UnityEngine;

namespace Mitochondria.Resources.FFmpeg.Utilities;

public static class AudioClipUtils
{
    public static async Task<AudioClip> CreateAudioClipAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        // TODO: handle endianness?

        var samples = await AudioConverter.ToPcmFloat32BitLittleEndianAsync(filePath, cancellationToken);
        var metadata = await GetAudioMetadataAsync(filePath, cancellationToken);
        var name = metadata.Title ?? Path.GetFileName(filePath);

        return CreateAudioClip(name, samples, metadata);
    }

    /// <remarks>
    /// This funky method ensures all data from the input stream has been copied before actually doing anything in order
    /// to circumvent the stream closing prematurely.
    /// </remarks>
    public static Task<AudioClip> CreateAudioClipAsync(
        Stream inputStream,
        CancellationToken cancellationToken = default)
    {
        if (!inputStream.CanSeek)
        {
            throw new ArgumentException("Stream must be seekable", nameof(inputStream));
        }

        var memoryStream = new MemoryStream();
        inputStream.CopyTo(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return CreateAudioClipAsyncInternal(memoryStream, cancellationToken).ContinueWith(
            t =>
            {
                memoryStream.Dispose();
                return t.Result;
            },
            cancellationToken);
    }

    public static AudioClip CreateAudioClip(string name, float[] samples, AudioMetadata metadata)
    {
        var audioClip = AudioClip.Create(name, samples.Length, metadata.Channels, metadata.SampleRate, false, false);
        audioClip.SetData(samples, 0);

        return audioClip;
    }

    public static async Task<AudioMetadata> GetAudioMetadataAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var info = await FFmpegUtils.GetAudioFileInfoAsync(filePath, cancellationToken);
        return ParseAudioMetadata(info);
    }

    public static async Task<AudioMetadata> GetAudioMetadataAsync(
        Stream inputStream,
        CancellationToken cancellationToken = default)
    {
        var info = await FFmpegUtils.GetAudioFileInfoAsync(inputStream, cancellationToken);
        return ParseAudioMetadata(info);
    }

    private static async Task<AudioClip> CreateAudioClipAsyncInternal(
        Stream inputStream,
        CancellationToken cancellationToken = default)
    {
        // TODO: handle endianness?

        var samples = await AudioConverter.ToPcmFloat32BitLittleEndianAsync(inputStream, cancellationToken);

        inputStream.Seek(0, SeekOrigin.Begin);
        var metadata = await GetAudioMetadataAsync(inputStream, cancellationToken);

        var name = metadata.Title ?? "UNTITLED";
        return CreateAudioClip(name, samples, metadata);
    }

    private static AudioMetadata ParseAudioMetadata(Dictionary<string, string> info)
    {
        var channels = int.Parse(info["channels"]);
        var sampleRate = int.Parse(info["sample_rate"]);
        var title = info.GetValueOrDefault("tag:title");

        return new AudioMetadata(channels, sampleRate, title);
    }

    public record AudioMetadata(int Channels, int SampleRate, string? Title = null);
}
