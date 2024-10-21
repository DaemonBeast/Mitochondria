using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mitochondria.Core.Utilities.Extensions;

public static class SpanExtensions
{
    public static bool BlockCast<T>(
        this Memory<byte> source,
        Memory<T> destination,
        out ReadOnlyMemory<byte> excess,
        out int elementCount,
        out int castedByteCount)
        where T : struct
        => BlockCast((ReadOnlyMemory<byte>) source, destination, out excess, out elementCount, out castedByteCount);

    public static bool BlockCast<T>(
        this ReadOnlyMemory<byte> source,
        Memory<T> destination,
        out ReadOnlyMemory<byte> excess,
        out int elementCount,
        out int castedByteCount)
        where T : struct
    {
        var destinationElementSize = Unsafe.SizeOf<T>();

        elementCount = source.Length / destinationElementSize;
        castedByteCount = elementCount * destinationElementSize;

        source.Span[..castedByteCount].CopyTo(MemoryMarshal.AsBytes(destination.Span));

        if (castedByteCount == source.Length)
        {
            excess = ReadOnlyMemory<byte>.Empty;
            return false;
        }

        excess = source[castedByteCount..];
        return true;
    }
}
