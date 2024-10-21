using System.Diagnostics.CodeAnalysis;
using Mitochondria.Core.Utilities.Structures;
using Reactor.Localization.Utilities;

namespace Mitochondria.Localization;

public static class StringNameMapping
{
    private static readonly Map<string, StringNames> Map = new();

    public static StringNames GetStringName(string key)
    {
        if (Map.TryGetValue(key, out var stringName))
        {
            return stringName;
        }

        stringName = CustomStringName.Create();
        Map[key] = stringName;
        return stringName;
    }

    public static bool TryGetKey(StringNames stringName, [NotNullWhen(true)] out string? key)
        => Map.Reverse.TryGetValue(stringName, out key);
}
