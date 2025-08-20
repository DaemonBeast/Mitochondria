using System.Buffers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Mitochondria.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Resources.FFmpeg.Utilities;

public class StreamingAudioClip : IAsyncDisposable
{
    public AudioClipUtils.AudioMetadata Metadata { get; }

    public AudioClip AudioClip { get; private set; } = null!;

    public int PlaybackPosition { get; private set; }

    private SpinLock _spinLock;
    private int _writePosition;

    // TODO: make version with no caching? Would mean no seeking (maybe start new process at point seeked to?)
    private readonly IMemoryOwner<float> _dataOwner;
    private Memory<float>? _data;

    private bool _disposed;

    public static async Task<StreamingAudioClip> CreateAsync(string name, AudioClipUtils.AudioMetadata metadata)
    {
        var length = (int)
            (metadata.SampleRate * metadata.DurationTimestamp * metadata.TimeBase.Numerator /
             metadata.TimeBase.Denominator);

        var streamingAudioClip = new StreamingAudioClip(metadata, length);

        var audioClip = await AudioClipUtils.CreateAudioClipAsync(
            name,
            length,
            metadata.Channels,
            metadata.SampleRate,
            false,
            true,
            streamingAudioClip.OnAudioRead,
            streamingAudioClip.OnAudioSetPosition);

        streamingAudioClip.AudioClip = audioClip;

        return streamingAudioClip;
    }

    private StreamingAudioClip(AudioClipUtils.AudioMetadata metadata, int length)
    {
        Metadata = metadata;

        _dataOwner = MemoryPool<float>.Shared.Rent(length * metadata.Channels);
        _data = _dataOwner.Memory;
        _data.Value.Span.Fill(0f);
    }

    public void Write(float[] source, int length)
    {
        if (!_data.HasValue)
        {
            return;
        }

        if (length < 0 || length >= source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        var sourceSpan = new ReadOnlySpan<float>(source, 0, length);

        var destination = _data.Value.Span.Slice(_writePosition, length);
        _writePosition += length;

        var lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);
            sourceSpan.CopyTo(destination);
        }
        finally
        {
            if (lockTaken)
            {
                _spinLock.Exit();
            }
        }
    }

    public ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return ValueTask.CompletedTask;
        }

        _disposed = true;
        GC.SuppressFinalize(this);

        _dataOwner.Dispose();
        _data = null;

        return AudioClip == null ? ValueTask.CompletedTask : new ValueTask(AudioClip.DestroyAsync());
    }

    private void OnAudioRead(Il2CppStructArray<float> data)
    {
        if (!_data.HasValue)
        {
            data.AsSpan().Fill(0f);
            return;
        }

        var length = data.Length;
        var source = _data.Value.Span.Slice(PlaybackPosition, length);
        PlaybackPosition += length;

        var destination = data.AsSpan();

        var lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);
            source.CopyTo(destination);
        }
        catch
        {
            if (lockTaken)
            {
                lockTaken = false;
                _spinLock.Exit();
            }

            data.AsSpan().Fill(0f);
        }
        finally
        {
            if (lockTaken)
            {
                _spinLock.Exit();
            }
        }
    }

    private void OnAudioSetPosition(int newPosition)
        => PlaybackPosition = newPosition * Metadata.Channels;
}
