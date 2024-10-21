using System.Buffers;
using System.Runtime.CompilerServices;
using Mitochondria.Core.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Resources.FFmpeg.Utilities;

public static class AudioClipUtils
{
    public static async Task<AudioClip> CreateAudioClipAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var samples = await AudioConverter.ToPcmF32LeAsync(filePath, cancellationToken);
        var metadata = await GetAudioMetadataAsync(filePath, cancellationToken);
        var name = metadata.Title ?? Path.GetFileName(filePath);

        return CreateAudioClip(name, samples, metadata);
    }

    public static async Task<AudioClip> CreateAudioClipAsync(
        Stream inputStream,
        CancellationToken cancellationToken = default)
    {
        if (!inputStream.CanSeek)
        {
            throw new ArgumentException("Stream must be seekable", nameof(inputStream));
        }

        var samples = await AudioConverter.ToPcmF32LeAsync(inputStream, cancellationToken);

        inputStream.Seek(0, SeekOrigin.Begin);
        var metadata = await GetAudioMetadataAsync(inputStream, cancellationToken);

        var name = metadata.Title ?? "UNTITLED";
        return CreateAudioClip(name, samples, metadata);
    }

    public static AudioClip CreateAudioClip(string name, float[] samples, AudioMetadata metadata)
    {
        var audioClip = AudioClip.Create(name, samples.Length, metadata.Channels, metadata.SampleRate, false, false);
        audioClip.SetData(samples, 0);

        return audioClip;
    }

    public static async Task<AudioClip> CreateStreamingAudioClipAsync(
        string fileName,
        Func<ValueTask>? onEnd = null,
        CancellationToken cancellationToken = default)
    {
        var metadata = await GetAudioMetadataAsync(fileName, cancellationToken);

        var name = metadata.Title ?? fileName;
        var streamingAudioClip = new StreamingAudioClip(name, metadata);

        using var ffmpegOutputStreamOwner = AudioConverter.ToPcmF32LeStream(fileName);
        var ffmpegOutputStream = ffmpegOutputStreamOwner.Stream;

        return await CreateStreamingAudioClipAsyncInternal(
            streamingAudioClip,
            ffmpegOutputStream,
            onEnd,
            cancellationToken);
    }

    public static async Task<AudioClip> CreateStreamingAudioClipAsync(
        Stream inputStream,
        Func<ValueTask>? onEnd = null,
        CancellationToken cancellationToken = default)
    {
        if (!inputStream.CanSeek)
        {
            throw new ArgumentException("Stream must be seekable", nameof(inputStream));
        }

        // TODO: maybe pipe data into both at same time?
        var metadata = await GetAudioMetadataAsync(inputStream, cancellationToken);
        inputStream.Seek(0, SeekOrigin.Begin);

        var name = metadata.Title ?? "UNTITLED";
        var streamingAudioClip = new StreamingAudioClip(name, metadata);

        var ffmpegOutputStream = AudioConverter.ToPcmF32LeStream(inputStream, cancellationToken);

        return await CreateStreamingAudioClipAsyncInternal(
            streamingAudioClip,
            ffmpegOutputStream,
            onEnd,
            cancellationToken);
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

    private static async Task<AudioClip> CreateStreamingAudioClipAsyncInternal(
        StreamingAudioClip streamingAudioClip,
        Stream ffmpegOutputStream,
        Func<ValueTask>? onEnd = null,
        CancellationToken cancellationToken = default)
    {
        var taskCompletionSource = new TaskCompletionSource();

        _ = Task
            .Run(
                async () =>
                {
                    using var byteBufferOwner = MemoryPool<byte>.Shared.Rent(Core.Constants.BufferSize);
                    var byteBuffer = byteBufferOwner.Memory;
                    // Ensure the buffer can fit floats snugly to prevent excess bytes needing to be copied to the start
                    byteBuffer = byteBuffer[..^(byteBuffer.Length % sizeof(float))];

                    var position = 0;
                    var excess = ReadOnlyMemory<byte>.Empty;

                    var sampleBuffer = ArrayPool<float>.Shared.Rent(byteBuffer.Length / sizeof(float));
                    var sampleBufferMemory = sampleBuffer.AsMemory();
                    var sizeOfHalf = Unsafe.SizeOf<Half>();

                    try
                    {
                        int byteCount;
                        while ((byteCount = await ffmpegOutputStream.ReadAsync(
                                   byteBuffer[excess.Length..],
                                   cancellationToken)) > 0)
                        {
                            var hasExcess = byteBuffer[..byteCount].BlockCast(
                                sampleBufferMemory,
                                out excess,
                                out var elementCount,
                                out var castedByteCount);

                            for (var i = 0; i < castedByteCount / sizeOfHalf; i++)
                            {
                                // There's gotta be a better way
                                streamingAudioClip.Data[position + i] = (Half) sampleBuffer[i];
                            }

                            // Could probably move this out the loop, but then I'd have to duplicate code and ugh
                            if (taskCompletionSource is { } t)
                            {
                                _ = t.TrySetResult();
                                taskCompletionSource = null;
                            }

                            position += elementCount;

                            if (hasExcess)
                            {
                                excess.CopyTo(byteBuffer);
                            }
                        }
                    }
                    finally
                    {
                        ArrayPool<float>.Shared.Return(sampleBuffer);
                    }
                },
                cancellationToken)
            .ContinueWith(_ => onEnd?.Invoke(), cancellationToken);

        // Wait till there's some data before continuing
        await taskCompletionSource.Task;

        return streamingAudioClip.AudioClip;
    }

    private static AudioMetadata ParseAudioMetadata(Dictionary<string, string> info)
    {
        var channels = int.Parse(info["stream.0.channels"]);
        var sampleRate = int.Parse(info["stream.0.sample_rate"]);

        // TODO: may fail for some formats (e.g. webm) when streamed
        // This appears to be due to the stream duration being missing (to be expected since it's being streamed in so
        // the file metadata is unavailable) and because of the "packet.0" being missing; it seems that formats like
        // webm have much less packets than other formats and attempting to use "-read_intervals 999999999" with ffprobe
        // results in the last packet ("packet.0") to be missing from the results
        var durationTimestamp = info.TryGetValue("stream.0.duration_ts", out var d)
            ? long.Parse(d)
            : long.Parse(info["packet.0.pts"]) + long.Parse(info["packet.0.duration"]);

        var timeBaseParts = info["stream.0.time_base"].Split("/", 2);
        var timeBase = new TimeBase(int.Parse(timeBaseParts[0]), int.Parse(timeBaseParts[1]));

        var title = info.GetValueOrDefault("format.tags.title");

        return new AudioMetadata(channels, sampleRate, durationTimestamp, timeBase, title);
    }

    public record AudioMetadata(
        int Channels,
        int SampleRate,
        long DurationTimestamp,
        TimeBase TimeBase,
        string? Title = null);

    public record TimeBase(int Numerator, int Denominator);
}
