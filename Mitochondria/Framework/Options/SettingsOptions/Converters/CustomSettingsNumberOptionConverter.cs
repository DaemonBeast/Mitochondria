using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options.SettingsOptions;

namespace Mitochondria.Framework.Options.SettingsOptions.Converters;

[CustomSettingsOptionConverter]
public class CustomSettingsNumberOptionConverter : ICustomSettingsOptionConverter
{
    public Type ReturnedConvertedType => typeof(NumberOptionArgs);

    public Type OptionType => typeof(NumberOption);

    public bool Matches(Type customOptionType)
        => typeof(ICustomNumberOption).IsAssignableFrom(customOptionType);

    public bool TryConvert(ICustomSettingsOption customSettingsOption, [NotNullWhen(true)] out object? args)
    {
        if (customSettingsOption.BoxedCustomOption is not ICustomNumberOption customNumberOption)
        {
            args = null;
            return false;
        }

        args = new NumberOptionArgs(
            customNumberOption.TitleName,
            customNumberOption.Value,
            customNumberOption.Range,
            numberOption => customNumberOption.Value = numberOption.Value,
            customNumberOption.Step,
            customNumberOption.ZeroIsInfinity);

        return true;
    }
}