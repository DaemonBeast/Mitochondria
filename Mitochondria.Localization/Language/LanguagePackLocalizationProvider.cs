using Reactor.Localization;

namespace Mitochondria.Localization.Language;

public class LanguagePackLocalizationProvider : LocalizationProvider
{
    public LanguagePack LanguagePack { get; }

    public LanguagePackLocalizationProvider(LanguagePack languagePack)
    {
        LanguagePack = languagePack;
    }

    public override bool TryGetText(StringNames stringName, out string? result)
    {
        if (!StringNameMapping.TryGetKey(stringName, out var key))
        {
            result = null;
            return false;
        }

        return LanguagePack.TryGetTranslation(CurrentLanguage ?? SupportedLangs.English, key, out result);
    }
}
