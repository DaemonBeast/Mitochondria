using Mitochondria.Utilities.Structures;

namespace Mitochondria.Localization;

public static class LanguageCodeMapping
{
    public static Map<string, SupportedLangs> Languages { get; } =
        new()
        {
            ["en-US"] = SupportedLangs.English,
            ["es-419"] = SupportedLangs.Latam,
            ["pt-BR"] = SupportedLangs.Brazilian,
            ["pt"] = SupportedLangs.Portuguese,
            ["ko"] = SupportedLangs.Korean,
            ["ru"] = SupportedLangs.Russian,
            ["nl"] = SupportedLangs.Dutch,
            ["fil"] = SupportedLangs.Filipino,
            ["fr"] = SupportedLangs.French,
            ["de"] = SupportedLangs.German,
            ["it"] = SupportedLangs.Italian,
            ["ja"] = SupportedLangs.Japanese,
            ["es"] = SupportedLangs.Spanish,
            ["zh-Hans"] = SupportedLangs.SChinese,
            ["zh-Hant"] = SupportedLangs.TChinese,
            ["ga"] = SupportedLangs.Irish
        };
}
