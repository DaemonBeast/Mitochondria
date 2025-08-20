namespace Mitochondria.Utilities.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        => source.Where(e => e != null)!;
}
