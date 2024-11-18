using System.Diagnostics;

namespace Mitochondria.Resources.FFmpeg.Utilities;

public class FFmpegOutputStreamOwner : IFFmpegStreamOwner
{
    public Stream Stream { get; }

    private readonly Process _process;

    public FFmpegOutputStreamOwner(Process process)
    {
        _process = process;

        Stream = _process.StandardOutput.BaseStream;
    }

    public void Dispose()
    {
        _process.Dispose();
        GC.SuppressFinalize(this);
    }
}
