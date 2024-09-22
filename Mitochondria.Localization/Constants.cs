namespace Mitochondria.Localization;

public static class Constants
{
    public static class Paths
    {
        public static readonly string LocalizationCache =
            Path.Combine(BepInEx.Paths.CachePath, typeof(Constants).Assembly.GetName().Name!);

        public static readonly string LanguagePackCache = Path.Combine(LocalizationCache, "LanguagePacks");
    }
}
