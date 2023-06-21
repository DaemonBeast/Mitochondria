using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using Mitochondria.Api.Options.SettingsOptions;

namespace Mitochondria.Framework.Options.SettingsOptions;

public class CustomSettingsStringOption<TPlugin, TEnum> : CustomSettingsOption<TPlugin, TEnum>
    where TPlugin : BasePlugin
    where TEnum : struct, Enum
{
    public CustomSettingsStringOption(
        ICustomStringOption<TEnum> customStringOption,
        GameModes gameMode = GameModes.Normal,
        int? order = null) : base(customStringOption, gameMode, order)
    {
    }
}