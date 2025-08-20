using System.Net.Http.Json;
using System.Text.Json.Serialization;
using BepInEx;
using BepInEx.Unity.IL2CPP;

namespace Mitochondria.Utilities;

public static class GitHubUtilities<TPlugin>
    where TPlugin : BasePlugin
{
    public static HttpClient Client
    {
        get
        {
            var metadata = MetadataHelper.GetMetadata(typeof(TPlugin));
            var userAgent = $"{metadata.GUID}/{metadata.Version}";
            return _client ??= HttpUtilities.CreateGitHubClient(userAgent);
        }
    }

    // ReSharper disable once StaticMemberInGenericType
    private static HttpClient? _client;

    public static async Task<GitHubUtilities.RepositoryContent[]?> TryGetRepositoryContents(
        string owner,
        string repository,
        string subPath = "")
    {
        if (!Uri.TryCreate(
                new Uri($"https://api.github.com/repos/{owner}/{repository}/contents/"),
                subPath,
                out var contentUri))
        {
            return null;
        }

        try
        {
            return await Client.GetFromJsonAsync<GitHubUtilities.RepositoryContent[]>(contentUri);
        }
        catch
        {
            return null;
        }
    }
}

public static class GitHubUtilities
{
    public class RepositoryContent
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("download_url")]
        public string DownloadUrl { get; set; }

        public RepositoryContent(string name, string sha, string downloadUrl)
        {
            Name = name;
            Sha = sha;
            DownloadUrl = downloadUrl;
        }
    }
}
