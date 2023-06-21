using AmongUs.GameOptions;

namespace Mitochondria.Api.Options.SettingsOptions;

public interface ICustomSettingsOption<TValue> : ICustomSettingsOption
    where TValue : notnull
{
    public ICustomOption<TValue> CustomOption { get; }
}

public interface ICustomSettingsOption
{
    public GameModes GameMode { get; }
    
    public int? Order { get; }

    public ICustomOption BoxedCustomOption { get; }
    
    public delegate void ChangedHandler(ICustomSettingsOption customSettingsOption);

    public event ChangedHandler? OnChanged;
}