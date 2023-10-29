namespace Mitochondria.Core.Framework.Utilities.Extensions;

public static class BindingExtensions
{
    public static void Equalize<T>(this T controlValue, T valueToCompare, Action<T> valueChanged)
    {
        if (!EqualityComparer<T>.Default.Equals(controlValue, valueToCompare))
        {
            valueChanged.Invoke(valueToCompare);
        }
    }

    public static void Equalize<T>(
        this T controlValue,
        T valueToCompare,
        T otherValueToCompare,
        Action<T> valueChanged,
        Action<T> otherValueChanged)
        => Equalize(
            controlValue,
            valueToCompare,
            otherValueToCompare,
            valueChanged,
            otherValueChanged,
            EqualityComparer<T>.Default);

    public static void Equalize<T>(
        this T controlValue,
        T valueToCompare,
        T otherValueToCompare,
        Action<T> valueChanged,
        Action<T> otherValueChanged,
        EqualityComparer<T> equalityComparer)
    {
        if (equalityComparer.Equals(controlValue, valueToCompare) &&
            !equalityComparer.Equals(valueToCompare, otherValueToCompare))
        {
            otherValueChanged.Invoke(otherValueToCompare);
            return;
        }

        if (equalityComparer.Equals(controlValue, otherValueToCompare) &&
            !equalityComparer.Equals(valueToCompare, otherValueToCompare))
        {
            valueChanged.Invoke(valueToCompare);
            return;
        }

        if (!equalityComparer.Equals(controlValue, valueToCompare))
        {
            otherValueChanged.Invoke(otherValueToCompare);
            valueChanged.Invoke(valueToCompare);
        }
    }
}