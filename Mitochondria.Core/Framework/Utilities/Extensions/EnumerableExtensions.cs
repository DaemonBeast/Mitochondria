using I = Il2CppSystem.Collections.Generic;

namespace Mitochondria.Core.Framework.Utilities.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<(T, T)> Combine<T>(this T[] enumerable, T[] other)
    {
        var enumerableLength = enumerable.Length;
        var otherLength = other.Length;

        var lastCommonIndex = Math.Min(enumerableLength, otherLength) - 1;

        for (var i = 0; i <= lastCommonIndex; i++)
        {
            var enumerableI = enumerable[i];
            var otherI = other[i];

            for (var j = 0; j < i; j++)
            {
                var enumerableJ = enumerable[j];
                var otherJ = other[j];

                yield return (enumerableI, otherJ);
                yield return (enumerableJ, otherI);
            }

            yield return (enumerableI, otherI);
        }

        if (enumerableLength > otherLength)
        {
            var lastOther = other[^1];

            for (var i = lastCommonIndex + 1; i < enumerableLength; i++)
            {
                yield return (enumerable[i], lastOther);
            }
        }
        else
        {
            var lastEnumerable = enumerable[^1];

            for (var i = lastCommonIndex + 1; i < otherLength; i++)
            {
                yield return (lastEnumerable, other[i]);
            }
        }
    }

    public static bool IsOrdered<T>(this IEnumerable<T> enumerable, IComparer<T>? comparer = null)
    {
        var actualComparer = comparer ?? Comparer<T>.Default;

        using var enumerator = enumerable.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            // It's just empty
            return true;
        }

        var previous = enumerator.Current;
        while (enumerator.MoveNext())
        {
            if (actualComparer.Compare(enumerator.Current, previous) < 0)
            {
                return false;
            }

            previous = enumerator.Current;
        }

        return true;
    }

    public static I.List<T> ToIl2CppList<T>(this IEnumerable<T> enumerable)
    {
        var list = new I.List<T>();
        foreach (var item in enumerable)
        {
            list.Add(item);
        }

        return list;
    }
}