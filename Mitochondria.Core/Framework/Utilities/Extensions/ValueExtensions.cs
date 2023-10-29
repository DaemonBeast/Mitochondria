namespace Mitochondria.Core.Framework.Utilities.Extensions;

public static class ValueExtensions
{
    public static T? DefaultToNull<T>(this T value)
        where T : struct
    {
        return EqualityComparer<T>.Default.Equals(value, default) ? null : value;
    }
}