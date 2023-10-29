using AmongUs.GameOptions;
using Mitochondria.Core.Api.Owner;

namespace Mitochondria.Core.Api.Options.SettingsOptions;

public interface ICustomSettingsOption<TValue> : ICustomSettingsOption
    where TValue : notnull
{
    public ICustomOption<TValue> CustomOption { get; }
}

public interface ICustomSettingsOption : IOwned
{
    public GameModes GameMode { get; }
    
    public int? Order { get; }

    public ICustomOption BoxedCustomOption { get; }
    
    public delegate void ChangedHandler(ICustomSettingsOption customSettingsOption);

    public event ChangedHandler? OnChanged;
}