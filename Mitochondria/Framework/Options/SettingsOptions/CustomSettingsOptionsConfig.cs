using Mitochondria.Api.Configuration;
using YamlDotNet.Serialization;

namespace Mitochondria.Framework.Options.SettingsOptions;

public class CustomSettingsOptionsConfig : IConfig
{
    [YamlIgnore]
    public string ConfigName => "Custom Settings";

    [YamlMember(Alias = "Custom Settings")]
    public Dictionary<string, object> CustomSettings { get; set; }

    public CustomSettingsOptionsConfig()
    {
        CustomSettings = new Dictionary<string, object>();
    }
}