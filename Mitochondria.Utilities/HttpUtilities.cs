using System.Net.Http.Headers;

namespace Mitochondria.Utilities;

public static class HttpUtilities
{
    public static HttpClient CreateGitHubClient(string userAgent)
    {
        var httpClient = new HttpClient();

        // TODO: sanitise user agent
        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(userAgent);

        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            $"{MitochondriaUtilitiesPlugin.Id}/{MitochondriaUtilitiesPlugin.Version}");

        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github.raw+json"));

        return httpClient;
    }
}
