using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using Mitochondria.Api.Options.SettingsOptions;

namespace Mitochondria.Framework.Options.SettingsOptions;

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

public readonly record struct ToggleOptionArgs(
    StringNames Title,
    bool Value,
    Action<ToggleOption>? ValueChangedHandler = null);