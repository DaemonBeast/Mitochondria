using System.Text.Json;
using Reactor.Utilities;

namespace Mitochondria.Localization.Language;

// TODO: add optional expiry to saved language packs
public static class LanguagePackManager
{
    public static async Task<LanguagePack?> TryLoad(string id, Func<SupportedLangs, bool>? shouldLoadLanguage = null)
    {
        try
        {
            return await Load(id, shouldLoadLanguage);
        }
        catch
        {
            return null;
        }
    }

    public static async Task<bool> TrySave(
        LanguagePack languagePack,
        Func<SupportedLangs, bool>? shouldSaveLanguage = null)
    {
        try
        {
            await Save(languagePack, shouldSaveLanguage);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<LanguagePack?> Load(string id, Func<SupportedLangs, bool>? shouldLoadLanguage = null)
    {
        var shouldLoadLanguageInternal = shouldLoadLanguage ?? (_ => true);

        var languagePackPath = GetLanguagePackPath(id);
        if (!Directory.Exists(languagePackPath))
        {
            return null;
        }

        if (await LoadOnlyMetadata(id) is not { } languagePack)
        {
            return null;
        }

        var languageFiles = new List<LanguageFile>();

        foreach (var language in LanguageCodeMapping.Languages.Values)
        {
            if (!shouldLoadLanguageInternal.Invoke(language))
            {
                continue;
            }

            if (await TryLoadLanguageFile(languagePackPath, language) is { } languageFile)
            {
                languageFiles.Add(languageFile);
            }
            else
            {
                Logger<MitochondriaLocalizationPlugin>.Error($"Failed to load language file from cache");
            }
        }

        foreach (var languageFile in languageFiles)
        {
            languagePack.LanguageFiles.Add(languageFile.Language, languageFile);
        }

        return languagePack;
    }

    public static async Task<LanguagePack?> LoadOnlyMetadata(string languagePackId)
    {
        var languagePackPath = GetLanguagePackPath(languagePackId);
        if (!Directory.Exists(languagePackPath))
        {
            return null;
        }

        var metadataPath = GetMetadataPath(languagePackPath);
        if (!File.Exists(metadataPath))
        {
            return null;
        }

        LanguagePackMetadata metadata;
        await using (var metadataFileStream = File.OpenRead(metadataPath))
        {
            metadata = (await JsonSerializer.DeserializeAsync<LanguagePackMetadata>(metadataFileStream))!;
        }

        return new LanguagePack(languagePackId, metadata);
    }

    public static async Task<LanguageFile?> TryLoadLanguageFile(string languagePackId, SupportedLangs language)
    {
        var languageCode = LanguageCodeMapping.Languages.Reverse[language];
        var languagePackPath = GetLanguagePackPath(languagePackId);
        var languageFilePath = Path.Combine(languagePackPath, $"{languageCode}.json");
        if (!File.Exists(languageFilePath))
        {
            return null;
        }

        await using var languageFileStream = File.OpenRead(languageFilePath);

        try
        {
            var translations =
                (await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(languageFileStream))!;

            return new LanguageFile(language, translations);
        }
        catch
        {
            return null;
        }
    }

    public static async Task Save(LanguagePack languagePack, Func<SupportedLangs, bool>? shouldSaveLanguage = null)
    {
        var shouldSaveLanguageInternal = shouldSaveLanguage ?? (_ => true);

        var languagePackPath = GetLanguagePackPath(languagePack.Id);
        Directory.CreateDirectory(languagePackPath);

        var metadataPath = GetMetadataPath(languagePackPath);
        await using (var metadataFileStream = File.OpenWrite(metadataPath))
        {
            await JsonSerializer.SerializeAsync(metadataFileStream, languagePack.Metadata);
        }

        foreach (var languageFile in languagePack.LanguageFiles.Values)
        {
            if (!shouldSaveLanguageInternal.Invoke(languageFile.Language))
            {
                continue;
            }

            var languageCode = LanguageCodeMapping.Languages.Reverse[languageFile.Language];
            var languageFilePath = Path.Combine(languagePackPath, $"{languageCode}.json");

            await using var languageFileStream = File.OpenWrite(languageFilePath);
            try
            {
                await JsonSerializer.SerializeAsync(languageFileStream, languageFile.Translations);
            }
            catch (NotSupportedException e)
            {
                Logger<MitochondriaLocalizationPlugin>.Error($"Failed to save language file to cache: {e}");
            }
        }
    }

    private static string GetMetadataPath(string languagePackPath)
        => Path.Combine(languagePackPath, "metadata.json");

    private static string GetLanguagePackPath(string languagePackId)
        => Path.Combine(Constants.Paths.LanguagePackCache, languagePackId);
}
