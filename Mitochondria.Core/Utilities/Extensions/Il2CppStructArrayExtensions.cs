using System.Runtime.CompilerServices;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Mitochondria.Core.Utilities.Structures;

namespace Mitochondria.Core.Utilities.Extensions;

public static class Il2CppStructArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<T> AsSpan<T>(this Il2CppStructArray<T> source, int offset, int count)
        where T : unmanaged
    {
        if (offset < 0 || offset >= source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        if (count < 0 || offset + count > source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        return new Span<T>((T*) source.Pointer + 4 * IntPtr.Size + offset, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<T> AsSpan<T>(this Il2CppStructArray<T> source)
        where T : unmanaged
        => new((T*) source.Pointer + 4 * IntPtr.Size, source.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Memory<T> AsMemory<T>(this Il2CppStructArray<T> source, int offset, int count)
        where T : unmanaged
    {
        if (offset < 0 || offset >= source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        if (count < 0 || offset + count > source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        return new UnmanagedMemoryManager<T>((T*) source.Pointer + 4 * IntPtr.Size + offset, count).Memory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Memory<T> AsMemory<T>(this Il2CppStructArray<T> source)
        where T : unmanaged
        => new UnmanagedMemoryManager<T>((T*) source.Pointer + 4 * IntPtr.Size, source.Length).Memory;
}
