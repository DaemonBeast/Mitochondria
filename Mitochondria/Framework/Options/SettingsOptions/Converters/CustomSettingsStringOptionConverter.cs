using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options.SettingsOptions;

namespace Mitochondria.Framework.Options.SettingsOptions.Converters;

[CustomSettingsOptionConverter]
public class CustomSettingsStringOptionConverter : ICustomSettingsOptionConverter
{
    public Type ReturnedConvertedType => typeof(StringOptionArgs);

    public Type OptionType => typeof(StringOption);

    public bool Matches(Type customOptionType)
        => typeof(ICustomStringOption).IsAssignableFrom(customOptionType);

    public bool TryConvert(ICustomSettingsOption customSettingsOption, [NotNullWhen(true)] out object? args)
    {
        if (customSettingsOption.BoxedCustomOption is not ICustomStringOption customStringOption)
        {
            args = null;
            return false;
        }

        args = new StringOptionArgs(
            customStringOption.TitleName,
            customStringOption.ValueInt,
            customStringOption.ValueNames,
            stringOption => customStringOption.ValueInt = stringOption.Value);

        return true;
    }
}