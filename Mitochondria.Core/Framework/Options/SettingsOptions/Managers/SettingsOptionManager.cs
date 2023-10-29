using Mitochondria.Core.Framework.UI.Flex.SettingsOptions;
using Mitochondria.Core.Framework.Utilities;

namespace Mitochondria.Core.Framework.Options.SettingsOptions.Managers;

public class SettingsOptionManager
{
    public static SettingsOptionManager Instance => Singleton<SettingsOptionManager>.Instance;
    
    public SettingsOptionFlexContainer FlexContainer { get; }

    private SettingsOptionManager()
    {
        FlexContainer = new SettingsOptionFlexContainer();
    }
}