using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options.SettingsOptions;
using UnityEngine;

namespace Mitochondria.Framework.Options.SettingsOptions.Converters;

[SettingsOptionConverter]
public class SettingsNumberOptionConverter : ISettingsOptionConverter
{
    public bool CanConvert(Type typeToConvert)
        => typeof(ICustomSettingsOption<float>).IsAssignableFrom(typeToConvert);

    public bool ConvertsTo(Type convertedType)
        => convertedType.IsAssignableFrom(typeof(NumberOption));

    public bool TryConvert(
        object objToConvert,
        [NotNullWhen(true)] out OptionBehaviour? settingsOption,
        Transform? parent = null)
    {
        if (objToConvert is not ICustomSettingsOption<float> { CustomOption: ICustomNumberOption customNumberOption } ||
            !SettingsOptionPrototypeManager.Instance.TryCloneAndGet<NumberOption>(out var numberOption, parent))
        {
            settingsOption = null;
            return false;
        }

        numberOption.Title = customNumberOption.TitleName;
        numberOption.TitleText.text = customNumberOption.Title;
        numberOption.Value = customNumberOption.Value;
        numberOption.ValidRange = customNumberOption.Range;
        numberOption.Increment = customNumberOption.Step;
        numberOption.ZeroIsInfinity = customNumberOption.ZeroIsInfinity;

        SettingsOptionConverterHelper.Bind(
            numberOption,
            customNumberOption,
            () => customNumberOption.Value = numberOption.Value,
            () => numberOption.Value = customNumberOption.Value);

        settingsOption = numberOption;
        return true;
    }
}