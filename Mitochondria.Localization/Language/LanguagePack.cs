using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using BepInEx.Unity.IL2CPP;
using Mitochondria.Core.Utilities;
using Mitochondria.Core.Utilities.Extensions;
using Reactor.Utilities;

namespace Mitochondria.Localization.Language;

public class LanguagePack
{
    public string Id { get; }

    public LanguagePackMetadata Metadata { get; }

    public Dictionary<SupportedLangs, LanguageFile> LanguageFiles { get; } = new();

    public static async Task<LanguagePack?> TryCreateFromGitHub<TPlugin>(
        string languagePackId,
        string repositoryOwner,
        string repositoryName,
        string repositorySubPath = "",
        bool ignoreCache = false,
        bool fallbackToCache = true)
        where TPlugin : BasePlugin
    {
        try
        {
            var languagePack = await CreateFromGitHub<TPlugin>(
                languagePackId,
                repositoryOwner,
                repositoryName,
                repositorySubPath,
                ignoreCache);

            if (languagePack != null)
            {
                return languagePack;
            }
        }
        catch
        {
            // ignored
        }

        return fallbackToCache ? await LanguagePackManager.Instance.TryLoad(languagePackId) : null;
    }

    public static async Task<LanguagePack?> CreateFromGitHub<TPlugin>(
        string languagePackId,
        string repositoryOwner,
        string repositoryName,
        string repositorySubPath = "",
        bool ignoreCache = false)
        where TPlugin : BasePlugin
    {
        if (await GitHubUtils<TPlugin>.TryGetRepositoryContents(repositoryOwner, repositoryName, repositorySubPath)
            is not { } repositoryContents)
        {
            Logger<MitochondriaLocalizationPlugin>.Error("Failed to get GitHub repository contents for language pack");
            return null;
        }

        if (repositoryContents.FirstOrDefault(r => r.Name == "metadata.json") is not { } metadataContent)
        {
            Logger<MitochondriaLocalizationPlugin>.Error("Metadata file missing from GitHub language pack");
            return null;
        }

        var gitHubPath = Path.Combine(Constants.Paths.LocalizationCache, "GitHub");
        Directory.CreateDirectory(gitHubPath);

        var cacheDataPath = Path.Combine(gitHubPath, $"{languagePackId}.json");

        var cacheData = ignoreCache
            ? new Dictionary<string, string>()
            : await JsonUtils.DeserializeAsyncOrNew(cacheDataPath, () => new Dictionary<string, string>());

        var languageFiles = new List<LanguageFile>();
        var languageFileTasks = new List<Task<LanguageFile?>>();

        foreach (var repositoryContent in repositoryContents)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(repositoryContent.Name);

            if (!LanguageCodeMapping.Languages.TryGetValue(fileNameWithoutExtension, out var language))
            {
                continue;
            }

            if (cacheData.TryGetValue(fileNameWithoutExtension, out var fileHash) &&
                fileHash == repositoryContent.Sha &&
                await LanguagePackManager.Instance.TryLoadLanguageFile(languagePackId, language) is { } languageFile)
            {
                languageFiles.Add(languageFile);
                continue;
            }

            var languageFileTask = DownloadRawGitHubFile<TPlugin>(language, repositoryContent.DownloadUrl);
            languageFileTasks.Add(languageFileTask);
            cacheData[fileNameWithoutExtension] = repositoryContent.Sha;
        }

        // TODO: Should probably log an error if language file can't be downloaded
        var downloadedLanguageFiles = (await Task.WhenAll(languageFileTasks)).WhereNotNull().ToArray();
        var downloadedLanguages = downloadedLanguageFiles.Select(l => l.Language);
        languageFiles.AddRange(downloadedLanguageFiles);

        LanguagePack languagePack;
        if (cacheData.TryGetValue("metadata", out var metadataHash) &&
            metadataHash == metadataContent.Sha &&
            await LanguagePackManager.Instance.LoadOnlyMetadata(languagePackId) is { } pack)
        {
            languagePack = pack;
        }
        else
        {
            LanguagePackMetadata metadata;

            try
            {
                metadata = (await GitHubUtils<TPlugin>.Client
                    .GetFromJsonAsync<LanguagePackMetadata>(metadataContent.DownloadUrl))!;
            }
            catch
            {
                Logger<MitochondriaLocalizationPlugin>.Error(
                    "Failed to download or deserialize GitHub language pack metadata");

                return null;
            }

            languagePack = new LanguagePack(languagePackId, metadata);
        }

        foreach (var languageFile in languageFiles)
        {
            languagePack.LanguageFiles.Add(languageFile.Language, languageFile);
        }

        if (await LanguagePackManager.Instance.TrySave(languagePack, downloadedLanguages.Contains))
        {
            await using var cacheDataStream = File.OpenWrite(cacheDataPath);
            await JsonSerializer.SerializeAsync(cacheDataStream, cacheData);
        }

        return languagePack;
    }

    public LanguagePack(string id, LanguagePackMetadata metadata)
    {
        Id = id;
        Metadata = metadata;
    }

    public bool TryGetTranslation(SupportedLangs language, string fullKey, [NotNullWhen(true)] out string? translation)
    {
        var prefixLength = Metadata.Prefix.Length;
        var prefixPlusSeparatorLength = prefixLength + 1;

        if (prefixPlusSeparatorLength <= prefixLength ||
            fullKey[..prefixLength] != Metadata.Prefix ||
            !LanguageFiles.TryGetValue(language, out var languageFile))
        {
            translation = null;
            return false;
        }

        var subKey = fullKey[prefixPlusSeparatorLength..];
        return languageFile.Translations.TryGetValue(subKey, out translation);
    }

    private static async Task<LanguageFile?> DownloadRawGitHubFile<TPlugin>(
        SupportedLangs language,
        string downloadUrl)
        where TPlugin : BasePlugin
    {
        var fileStream = await GitHubUtils<TPlugin>.Client.GetStreamAsync(downloadUrl);
        return await LanguageFile.TryParse(language, fileStream);
    }
}
