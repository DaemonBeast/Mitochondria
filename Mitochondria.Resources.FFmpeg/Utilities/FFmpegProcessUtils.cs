using System.Buffers;
using System.Diagnostics;

namespace Mitochondria.Resources.FFmpeg.Utilities;

public static class FFmpegProcessUtils
{
    public static async Task<byte[]> PipeFullyAsync(
        Process process,
        Stream inputStream,
        CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        await PipeAsync(process, inputStream, memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }

    public static async Task PipeAsync(
        Process process,
        Stream inputStream,
        Stream outputStream,
        CancellationToken cancellationToken = default)
    {
        var processInputStream = process.StandardInput.BaseStream;

        var inputTask = Task.Run(
            async () =>
            {
                using var bufferOwner = MemoryPool<byte>.Shared.Rent(81920);
                var buffer = bufferOwner.Memory;
                var exceptionCount = 0;
                int count;

                while ((count = await inputStream.ReadAsync(buffer, cancellationToken)) != 0)
                {
                    try
                    {
                        await processInputStream.WriteAsync(buffer[..count], cancellationToken);
                    }
                    catch (IOException) when (++exceptionCount == 1)
                    {
                        // The first write causes a broken pipe exception to be thrown, but ignoring it appears to be
                        // fine, so unless you can fix it, this exists :)
                    }
                }

                await processInputStream.FlushAsync(cancellationToken);
                processInputStream.Close();
            },
            cancellationToken);

        var outputTask = Task.Run(
            async () => await process.StandardOutput.BaseStream.CopyToAsync(outputStream, cancellationToken),
            cancellationToken);

        await inputTask;
        await outputTask;
    }
}
