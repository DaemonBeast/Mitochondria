using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Resources.FFmpeg.Utilities;

public class StreamingAudioClip : IDisposable
{
    public AudioClipUtils.AudioMetadata Metadata { get; }

    public AudioClip AudioClip { get; }

    // TODO: make version with no caching? Would mean no seeking (maybe start new process at point seeked to?)
    // TODO: make version with floats instead of halves to improve quality while sacrificing memory?
    public Half[] Data { get; private set; }

    public int PlaybackPosition { get; private set; }

    public bool AtEnd => PlaybackPosition >= Data.Length;

    public event Action? Ended;

    public StreamingAudioClip(string name, AudioClipUtils.AudioMetadata metadata)
    {
        Metadata = metadata;

        var length = (int)
            (metadata.SampleRate * metadata.DurationTimestamp * metadata.TimeBase.Numerator /
             metadata.TimeBase.Denominator);

        Data = new Half[length * metadata.Channels];
        Array.Fill(Data, (Half) 0);

        AudioClip = AudioClip.Create(
            name,
            length,
            metadata.Channels,
            metadata.SampleRate,
            false,
            true,
            (Action<Il2CppStructArray<float>>) OnAudioRead,
            (Action<int>) OnAudioSetPosition);
    }

    public void Dispose()
    {
        if (AudioClip != null)
        {
            AudioClip.Destroy();
        }

        Data = null!;

        GC.SuppressFinalize(this);
    }

    private void OnAudioRead(Il2CppStructArray<float> data)
    {
        var containedLength = Math.Min(data.Length, Math.Max(0, Data.Length - PlaybackPosition));

        for (var i = 0; i < containedLength; i++)
        {
            data[i] = (float) Data[PlaybackPosition++];
        }

        if (containedLength == data.Length)
        {
            return;
        }

        for (var i = containedLength; i < data.Length; i++)
        {
            data[i] = 0;
        }

        Ended?.Invoke();
    }

    private void OnAudioSetPosition(int newPosition)
        => PlaybackPosition = newPosition;
}
