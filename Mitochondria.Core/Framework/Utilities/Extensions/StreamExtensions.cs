using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace Mitochondria.Core.Framework.Utilities.Extensions;

public static class StreamExtensions
{
    public static unsafe Il2CppStructArray<byte> AsBytes(this Stream stream)
    {
        var length = stream.Length;
        var array = new Il2CppStructArray<byte>(length);

        var span = new Span<byte>(IntPtr.Add(array.Pointer, IntPtr.Size * 4).ToPointer(), array.Length);
        _ = stream.Read(span);

        return array;
    }
}