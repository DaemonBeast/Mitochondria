using System.Buffers;
using System.Diagnostics;
using Mitochondria.Core.Utilities.Extensions;

namespace Mitochondria.Resources.FFmpeg.Utilities;

public static class FFmpegProcessUtils
{
    public static async Task<byte[]> PipeFullyAsync(
        Process process,
        Stream inputStream,
        CancellationToken cancellationToken = default)
    {
        var inputTask = Task.Run(
            () => CopyToStandardInputAsync(process, inputStream, cancellationToken),
            cancellationToken);

        var output = await process.StandardOutput.BaseStream.ReadFullyAsync(cancellationToken);

        await inputTask;
        return output;
    }

    public static async Task CopyToStandardInputAsync(
        Process process,
        Stream inputStream,
        CancellationToken cancellationToken = default)
    {
        var processInputStream = process.StandardInput.BaseStream;

        using var bufferOwner = MemoryPool<byte>.Shared.Rent(Core.Constants.BufferSize);
        var buffer = bufferOwner.Memory;
        var exceptionCount = 0;
        int count;

        while ((count = await inputStream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            try
            {
                await processInputStream.WriteAsync(buffer[..count], cancellationToken);
            }
            catch (IOException) when (++exceptionCount == 1)
            {
                // The first write causes a broken pipe exception to be thrown, but ignoring it appears to be fine, so
                // unless you know a fix, have this friendly message :)
            }
        }

        await processInputStream.FlushAsync(cancellationToken);
    }

    /// <remarks>
    /// A rudimentary attempt at preventing injection attacks in a probably already insecure environment.
    /// Internal access only because nobody else should ever be calling this.
    /// </remarks>
    internal static string VeryPoorlySanitizeFileName(string fileName)
        => fileName.Replace("\"", "\\\"");
}
