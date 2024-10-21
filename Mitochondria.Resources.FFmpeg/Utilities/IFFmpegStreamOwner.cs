namespace Mitochondria.Resources.FFmpeg.Utilities;

public interface IFFmpegStreamOwner : IDisposable
{
    public Stream Stream { get; }
}
