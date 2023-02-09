using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options;
using Mitochondria.Api.Options.SettingsOptions;

namespace Mitochondria.Framework.Options.SettingsOptions.Converters;

[CustomSettingsOptionConverter]
public class CustomSettingsToggleOptionConverter : ICustomSettingsOptionConverter
{
    public Type ReturnedConvertedType => typeof(ToggleOptionArgs);

    public Type OptionType => typeof(ToggleOption);

    public bool Matches(Type customOptionType)
        => typeof(ICustomOption<bool>).IsAssignableFrom(customOptionType);

    public bool TryConvert(ICustomSettingsOption customSettingsOption, [NotNullWhen(true)] out object? args)
    {
        if (customSettingsOption.BoxedCustomOption is not ICustomOption<bool> customToggleOption)
        {
            args = null;
            return false;
        }

        args = new ToggleOptionArgs(
            customToggleOption.TitleName,
            customToggleOption.Value,
            toggleOption => customToggleOption.Value = toggleOption.CheckMark.enabled);

        return true;
    }
}