using Mitochondria.Core.Framework.GUI.Extensions;
using Mitochondria.Core.Framework.Helpers;
using Mitochondria.Core.Framework.Utilities;
using Mitochondria.Core.Framework.Utilities.Extensions;
using Mitochondria.Options.Settings.Framework.Gui;

namespace Mitochondria.Options.Settings.Framework.Managers;

public class SettingsOptionManager
{
    public static SettingsOptionManager Instance => Singleton<SettingsOptionManager>.Instance;

    public SettingsOptionContainer? Container => GameOptionsMenuHelper.Current
        .AsNullable()?.gameObject
        .AsNullable()?.GetOrSetGuiContainer<SettingsOptionContainer>();
}