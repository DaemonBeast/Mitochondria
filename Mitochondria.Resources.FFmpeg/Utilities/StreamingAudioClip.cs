using System.Buffers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Mitochondria.Core.Utilities.Extensions;
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
    private readonly IMemoryOwner<Half> _dataOwner;
    private Memory<Half>? _data;

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

        _dataOwner = MemoryPool<Half>.Shared.Rent(length * metadata.Channels);
        _data = _dataOwner.Memory;
        _data.Value.Span.Fill((Half) 0);
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

        Span<Half> sourceHalfSpan = stackalloc Half[length];
        var destinationHalfSpan = _data.Value.Span.Slice(_writePosition, length);

        _writePosition += length;

        for (var i = 0; i < length; i++)
        {
            sourceHalfSpan[i] = (Half) source[i];
        }

        var lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);
            sourceHalfSpan.CopyTo(destinationHalfSpan);
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

        Span<float> sourceFloatSpan = stackalloc float[length];
        var sourceHalfSpan = _data.Value.Span.Slice(PlaybackPosition, length);
        var destinationFloatSpan = data.AsSpan();

        PlaybackPosition += length;

        for (var i = 0; i < length; i++)
        {
            sourceFloatSpan[i] = (float) sourceHalfSpan[i];
        }

        var lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);
            sourceFloatSpan.CopyTo(destinationFloatSpan);
        }
        finally
        {
            if (lockTaken)
            {
                _spinLock.Exit();
            }
            else
            {
                data.AsSpan().Fill(0f);
            }
        }
    }

    private void OnAudioSetPosition(int newPosition)
        => PlaybackPosition = newPosition * Metadata.Channels;
}
