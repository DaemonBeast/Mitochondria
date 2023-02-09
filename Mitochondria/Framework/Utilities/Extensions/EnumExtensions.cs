namespace Mitochondria.Framework.Utilities.Extensions;

public static class EnumExtensions
{
    public static int GetIndex<TEnum>(this TEnum value)
        where TEnum : struct, Enum
    {
        return Array.IndexOf(Enum.GetValues<TEnum>(), value);
    }

    public static TEnum? GetEnumFromIndex<TEnum>(this int index)
        where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>().ElementAtOrDefault(index);
    }
}