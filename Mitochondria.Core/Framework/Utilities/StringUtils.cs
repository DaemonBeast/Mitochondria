namespace Mitochondria.Core.Framework.Utilities;

public static class StringUtils
{
    public static string GetFormatString(string prefix = "", string postfix = "")
    {
        return $"{prefix}{{0}}{postfix}";
    }

    public static string GetFloatFormatString(int minDecimalPlaces, int maxDecimalPlaces)
    {
        if (maxDecimalPlaces < minDecimalPlaces)
        {
            throw new ArgumentException(
                $"{nameof(maxDecimalPlaces)} cannot be smaller than {nameof(minDecimalPlaces)}");
        }

        return $"{{0:0.{new string('0', minDecimalPlaces)}{new string('#', maxDecimalPlaces - minDecimalPlaces)}}}";
    }

    public static string GetFloatFormatString(
        int minDecimalPlaces,
        int maxDecimalPlaces,
        // ReSharper disable once MethodOverloadWithOptionalParameter
        string prefix = "",
        NumberSuffixes suffix = NumberSuffixes.None)
    {
        var postfix = suffix switch
        {
            NumberSuffixes.Multiplier => "x",
            NumberSuffixes.Seconds => TranslationController.Instance.GetString(
                StringNames.GameSecondsAbbrev,
                string.Empty),
            _ => string.Empty
        };

        return GetFloatFormatString(minDecimalPlaces, maxDecimalPlaces, prefix, postfix);
    }

    public static string GetFloatFormatString(
        int minDecimalPlaces,
        int maxDecimalPlaces,
        // ReSharper disable once MethodOverloadWithOptionalParameter
        string prefix = "",
        string postfix = "")
    {
        return string.Format(
            GetFormatString(prefix, postfix),
            GetFloatFormatString(minDecimalPlaces, maxDecimalPlaces));
    }
}