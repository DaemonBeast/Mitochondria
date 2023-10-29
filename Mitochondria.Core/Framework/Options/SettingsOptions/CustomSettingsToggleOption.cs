using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using Mitochondria.Core.Api.Options.SettingsOptions;

namespace Mitochondria.Core.Framework.Options.SettingsOptions;

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