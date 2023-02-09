namespace Mitochondria.Framework.Utilities.Extensions;

public static class ValueExtensions
{
    public static T? AsNullable<T>(this T value)
        where T : struct
    {
        return EqualityComparer<T>.Default.Equals(value, default) ? null : value;
    }
}