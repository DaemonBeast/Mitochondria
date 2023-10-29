using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using Mitochondria.Core.Framework.Utilities.Extensions;

namespace Mitochondria.Core.Api.Options.SettingsOptions;

public class CustomSettingsOption<TPlugin, TValue> : ICustomSettingsOption<TValue>
    where TPlugin : BasePlugin
    where TValue : notnull
{
    public GameModes GameMode { get; }
    
    public int? Order { get; }

    public ICustomOption<TValue> CustomOption { get; }

    public ICustomOption BoxedCustomOption => CustomOption;

    public event ICustomSettingsOption.ChangedHandler? OnChanged;
    
    public static implicit operator CustomOption<TPlugin, TValue>(CustomSettingsOption<TPlugin, TValue> c) =>
        (CustomOption<TPlugin, TValue>) c.CustomOption;

    public CustomSettingsOption(
        ICustomOption<TValue> customOption,
        GameModes gameMode = GameModes.Normal,
        int? order = null)
    {
        GameMode = gameMode;
        Order = order;
        CustomOption = customOption;

        CustomOption.OnChanged += _ => OnChanged?.Invoke(this);

        this.SetOwner<TPlugin>();
    }
}