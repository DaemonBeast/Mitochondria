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