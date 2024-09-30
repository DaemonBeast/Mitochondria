using Mitochondria.Localization.Language;
using Reactor.Localization;

namespace Mitochondria.Localization.Utilities.Extensions;

public static class LanguagePackExtensions
{
    public static async Task<bool> RegisterInLocalizationManager(this Task<LanguagePack?> languagePackTask)
        => RegisterInLocalizationManager(await languagePackTask);

    public static bool RegisterInLocalizationManager(this LanguagePack? languagePack)
    {
        if (languagePack == null)
        {
            return false;
        }

        LocalizationManager.Register(new LanguagePackLocalizationProvider(languagePack));
        return true;
    }
}
