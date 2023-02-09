using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using Mitochondria.Api.Options.SettingsOptions;

namespace Mitochondria.Framework.Options.SettingsOptions;

public class CustomSettingsNumberOption<TPlugin> : CustomSettingsOption<TPlugin, float>
    where TPlugin : BasePlugin
{
    public CustomSettingsNumberOption(
        ICustomNumberOption customNumberOption,
        GameModes gameMode = GameModes.Normal,
        int? order = null) : base(customNumberOption, gameMode, order)
    {
    }
}

public readonly record struct NumberOptionArgs(
    StringNames Title,
    float Value,
    FloatRange Range,
    Action<NumberOption>? ValueChangedHandler = null,
    float Step = 1f,
    bool ZeroIsInfinity = false);