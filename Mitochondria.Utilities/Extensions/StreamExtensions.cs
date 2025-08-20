namespace Mitochondria.Utilities.Extensions;

public static class StreamExtensions
{
    public static async Task<byte[]> ReadFullyAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }
}
