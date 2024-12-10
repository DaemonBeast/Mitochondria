using System.Buffers;
using System.Collections;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Mitochondria.Core.Utilities.Extensions;
using Reactor.Utilities;
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
        var name = metadata.Title ?? Path.GetFileNameWithoutExtension(filePath);

        return await CreateAudioClipAsync(name, samples, metadata);
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
        return await CreateAudioClipAsync(name, samples, metadata);
    }

    public static async Task<AudioClip> CreateAudioClipAsync(string name, float[] samples, AudioMetadata metadata)
    {
        var taskCompletionSource = new TaskCompletionSource<AudioClip>();

        Coroutines.Start(
            CoCreateAudioClip(
                name,
                samples.Length,
                metadata.Channels,
                metadata.SampleRate,
                false,
                false,
                audioClip => taskCompletionSource.TrySetResult(audioClip)));

        var audioClip = await taskCompletionSource.Task;
        audioClip.SetData(samples, 0);
        return audioClip;
    }

    public static async Task<AudioClip> CreateAudioClipAsync(
        string name,
        int lengthSamples,
        int channels,
        int frequency,
        bool _3D,
        bool stream)
    {
        var taskCompletionSource = new TaskCompletionSource<AudioClip>();

        Coroutines.Start(
            CoCreateAudioClip(
                name,
                lengthSamples,
                channels,
                frequency,
                _3D,
                stream,
                audioClip => taskCompletionSource.TrySetResult(audioClip)));

        var audioClip = await taskCompletionSource.Task;
        return audioClip;
    }

    public static async Task<AudioClip> CreateAudioClipAsync(
        string name,
        int lengthSamples,
        int channels,
        int frequency,
        bool _3D,
        bool stream,
        Action<Il2CppStructArray<float>> onAudioRead,
        Action<int> onAudioSetPosition)
    {
        var taskCompletionSource = new TaskCompletionSource<AudioClip>();

        Coroutines.Start(
            CoCreateAudioClip(
                name,
                lengthSamples,
                channels,
                frequency,
                _3D,
                stream,
                onAudioRead,
                onAudioSetPosition,
                audioClip => taskCompletionSource.TrySetResult(audioClip)));

        var audioClip = await taskCompletionSource.Task;
        return audioClip;
    }

    public static async Task<StreamingAudioClip> CreateStreamingAudioClipAsync(
        string filePath,
        Func<ValueTask>? onStreamingStart = null,
        Func<ValueTask>? onStreamingEnd = null,
        CancellationToken cancellationToken = default)
    {
        var metadata = await GetAudioMetadataAsync(filePath, cancellationToken);

        var name = metadata.Title ?? Path.GetFileNameWithoutExtension(filePath);

        var ffmpegOutputStreamOwner = AudioConverter.ToPcmF32LeStream(filePath);
        var ffmpegOutputStream = ffmpegOutputStreamOwner.Stream;

        var streamingAudioClip = await StreamingAudioClip.CreateAsync(name, metadata);

        try
        {
            PipeIntoStreamingAudioClipAsync(
                streamingAudioClip,
                ffmpegOutputStream,
                onStreamingStart,
                () =>
                {
                    ffmpegOutputStreamOwner.Dispose();
                    return onStreamingEnd?.Invoke() ?? ValueTask.CompletedTask;
                },
                cancellationToken);

            return streamingAudioClip;
        }
        catch
        {
            await streamingAudioClip.DisposeAsync();
            throw;
        }
    }

    public static async Task<StreamingAudioClip> CreateStreamingAudioClipAsync(
        Stream inputStream,
        Func<ValueTask>? onStreamingStart = null,
        Func<ValueTask>? onStreamingEnd = null,
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

        var ffmpegOutputStream = AudioConverter.ToPcmF32LeStream(inputStream, cancellationToken);

        var streamingAudioClip = await StreamingAudioClip.CreateAsync(name, metadata);

        try
        {
            PipeIntoStreamingAudioClipAsync(
                streamingAudioClip,
                ffmpegOutputStream,
                onStreamingStart,
                onStreamingEnd,
                cancellationToken);

            return streamingAudioClip;
        }
        catch
        {
            await streamingAudioClip.DisposeAsync();
            throw;
        }
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

    private static IEnumerator CoCreateAudioClip(
        string name,
        int lengthSamples,
        int channels,
        int frequency,
        bool _3D,
        bool stream,
        Action<AudioClip> onEnd)
    {
        yield return null;
        onEnd.Invoke(AudioClip.Create(name, lengthSamples, channels, frequency, _3D, stream));
    }

    private static IEnumerator CoCreateAudioClip(
        string name,
        int lengthSamples,
        int channels,
        int frequency,
        bool _3D,
        bool stream,
        Action<Il2CppStructArray<float>> onAudioRead,
        Action<int> onAudioSetPosition,
        Action<AudioClip> onEnd)
    {
        yield return null;

        onEnd.Invoke(
            AudioClip.Create(name, lengthSamples, channels, frequency, _3D, stream, onAudioRead, onAudioSetPosition));
    }

    private static void PipeIntoStreamingAudioClipAsync(
        StreamingAudioClip streamingAudioClip,
        Stream ffmpegOutputStream,
        Func<ValueTask>? onStreamingStart = null,
        Func<ValueTask>? onStreamingEnd = null,
        CancellationToken cancellationToken = default)
    {
        var taskCompletionSource = new TaskCompletionSource();

        _ = taskCompletionSource.Task.ContinueWith(
            async _ =>
            {
                if (onStreamingStart != null)
                {
                    await onStreamingStart.Invoke();
                }
            },
            cancellationToken);

        var byteBufferOwner = MemoryPool<byte>.Shared.Rent(Constants.BufferSize);
        var byteBuffer = byteBufferOwner.Memory;
        // Ensure the buffer can fit floats snugly to prevent excess bytes needing to be copied to the start
        byteBuffer = byteBuffer[..^(byteBuffer.Length % sizeof(float))];

        var excess = ReadOnlyMemory<byte>.Empty;

        var sampleBuffer = ArrayPool<float>.Shared.Rent(byteBuffer.Length / sizeof(float));
        var sampleBufferMemory = sampleBuffer.AsMemory();

        _ = Task
            .Run(
                async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int byteCount;
                    while ((byteCount = await ffmpegOutputStream.ReadAsync(
                               byteBuffer[excess.Length..],
                               cancellationToken).AsTask().WaitAsync(cancellationToken)) > 0)
                    {
                        var hasExcess = byteBuffer[..byteCount].BlockCast(
                            sampleBufferMemory,
                            out excess,
                            out var elementCount,
                            out _);

                        streamingAudioClip.Write(sampleBuffer, elementCount);

                        // Could probably move this out the loop, but then I'd have to duplicate code and ugh
                        if (taskCompletionSource is { } t)
                        {
                            _ = t.TrySetResult();
                            taskCompletionSource = null;
                        }

                        if (hasExcess)
                        {
                            excess.CopyTo(byteBuffer);
                        }
                    }
                },
                CancellationToken.None)
            .ContinueWith(
                async _ =>
                {
                    byteBufferOwner.Dispose();
                    ArrayPool<float>.Shared.Return(sampleBuffer);

                    if (onStreamingEnd != null)
                    {
                        await onStreamingEnd.Invoke();
                    }
                },
                CancellationToken.None);
    }

    private static AudioMetadata ParseAudioMetadata(Dictionary<string, string> info)
    {
        var channels = int.Parse(info["stream.0.channels"]);
        var sampleRate = int.Parse(info["stream.0.sample_rate"]);

        var timeBaseParts = info["stream.0.time_base"].Split("/", 2);
        var timeBase = new TimeBase(int.Parse(timeBaseParts[0]), int.Parse(timeBaseParts[1]));

        var durationTimestamp = info.TryGetValue("stream.0.duration_ts", out var sd)
            ? long.Parse(sd)
            : info.TryGetValue("packet.0.pts", out var s) && info.TryGetValue("packet.0.duration", out var pd)
                ? long.Parse(s) + long.Parse(pd)
                : (long) (double.Parse(info["format.duration"]) * timeBase.Denominator / timeBase.Numerator);

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
