using BitConverter = System.BitConverter;

namespace Mitochondria.Core.Framework.Utilities.Extensions;

public static class ByteExtensions
{
    public static ulong ToUInt64(this IEnumerable<byte> bytes)
        => BitConverter.ToUInt64(bytes.ToArray());
}