using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using Mitochondria.Core.Framework.Options;
using Mitochondria.Options.Settings.Abstractions;

namespace Mitochondria.Options.Settings.Framework.Options;

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