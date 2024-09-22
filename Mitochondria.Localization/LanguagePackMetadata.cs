using System.Text.Json.Serialization;

namespace Mitochondria.Localization;

public class LanguagePackMetadata
{
    [JsonPropertyName("prefix")]
    public string Prefix { get; set; }

    [JsonPropertyName("global-fallback")]
    public string GlobalFallback { get; set; }

    public LanguagePackMetadata(string prefix, string globalFallback)
    {
        Prefix = prefix;
        GlobalFallback = globalFallback;
    }
}
