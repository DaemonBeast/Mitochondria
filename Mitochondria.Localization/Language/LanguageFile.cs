using System.Collections.Immutable;
using System.Text.Json;
using Reactor.Utilities;

namespace Mitochondria.Localization.Language;

public class LanguageFile
{
    public SupportedLangs Language { get; }

    public ImmutableDictionary<string, string> Translations { get; }

    public LanguageFile(SupportedLangs language, IEnumerable<KeyValuePair<string, string>> translations)
    {
        Language = language;
        Translations = translations.ToImmutableDictionary();
    }

    public static async ValueTask<LanguageFile?> TryParse(SupportedLangs language, Stream languageData)
    {
        try
        {
            return new LanguageFile(
                language,
                (await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(languageData))!);
        }
        catch (Exception e) when (e is JsonException or NotSupportedException)
        {
            Logger<MitochondriaLocalizationPlugin>.Error($"Failed to deserialize language file: {e}");
            return null;
        }
    }
}
