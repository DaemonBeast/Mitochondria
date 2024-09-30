using UnityEngine;

namespace Mitochondria.Resources.FFmpeg.Utilities;

public static class AudioClipUtils
{
    public static AudioClip CreateAudioClipFromPath(string path)
    {
        // TODO: handle endianness?
        var samples =
            AudioConverter.ToPcmFloat32BitLittleEndian(path, out var channels, out var sampleRate, out var title);

        var name = title ?? Path.GetFileName(path);
        var audioClip = AudioClip.Create(name, samples.Length, channels, sampleRate, false, false);
        audioClip.SetData(samples, 0);

        return audioClip;
    }
}
