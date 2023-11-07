using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using Mitochondria.Core.Framework.Options;
using Mitochondria.Options.Settings.Abstractions;

namespace Mitochondria.Options.Settings.Framework.Options;

public class CustomSettingsToggleOption<TPlugin> : CustomSettingsOption<TPlugin, bool>
    where TPlugin : BasePlugin
{
    public CustomSettingsToggleOption(
        CustomToggleOption<TPlugin> customToggleOption,
        GameModes gameMode = GameModes.Normal,
        int? order = null) : base(customToggleOption, gameMode, order)
    {
    }
}