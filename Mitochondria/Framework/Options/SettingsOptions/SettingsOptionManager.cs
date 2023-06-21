using Mitochondria.Framework.UI.Flex.SettingsOptions;
using Mitochondria.Framework.Utilities;

namespace Mitochondria.Framework.Options.SettingsOptions;

public class SettingsOptionManager
{
    public static SettingsOptionManager Instance => Singleton<SettingsOptionManager>.Instance;
    
    public SettingsOptionFlexContainer FlexContainer { get; }

    private SettingsOptionManager()
    {
        FlexContainer = new SettingsOptionFlexContainer();
    }
}