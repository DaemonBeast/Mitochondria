using Mitochondria.Core.Api.Configuration;
using YamlDotNet.Serialization;

namespace Mitochondria.Options.Settings.Framework;

public class CustomSettingsOptionConfig : IConfig
{
    [YamlIgnore]
    public string ConfigName => "Custom Settings";

    [YamlMember(Alias = "Custom Settings")]
    public Dictionary<string, object> CustomSettings { get; set; }

    public CustomSettingsOptionConfig()
    {
        CustomSettings = new Dictionary<string, object>();
    }
}