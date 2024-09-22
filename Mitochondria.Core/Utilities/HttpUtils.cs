using System.Net.Http.Headers;

namespace Mitochondria.Core.Utilities;

public static class HttpUtils
{
    public static HttpClient CreateGitHubClient(string userAgent)
    {
        var httpClient = new HttpClient();

        // TODO: sanitise user agent
        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(userAgent);

        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            $"{MitochondriaCorePlugin.Id}/{MitochondriaCorePlugin.Version}");

        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github.raw+json"));

        return httpClient;
    }
}
