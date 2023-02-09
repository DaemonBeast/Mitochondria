using System.Diagnostics.CodeAnalysis;

namespace Mitochondria.Api.Options.SettingsOptions;

public interface ICustomSettingsOptionConverter
{
    public Type ReturnedConvertedType { get; }
    
    public Type OptionType { get; }

    public bool Matches(Type customOptionType);

    public bool TryConvert(ICustomSettingsOption customSettingsOption, [NotNullWhen(true)] out object? args);
}