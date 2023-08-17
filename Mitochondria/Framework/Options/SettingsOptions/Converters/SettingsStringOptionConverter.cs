using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options.SettingsOptions;
using Mitochondria.Framework.Options.SettingsOptions.Managers;
using UnityEngine;

namespace Mitochondria.Framework.Options.SettingsOptions.Converters;

[SettingsOptionConverter]
public class SettingsStringOptionConverter : ISettingsOptionConverter
{
    public bool CanConvert(Type typeToConvert)
        => typeof(ICustomSettingsOption<string>).IsAssignableFrom(typeToConvert);

    public bool ConvertsTo(Type convertedType)
        => convertedType.IsAssignableFrom(typeof(StringOption));

    public bool TryConvert(
        object objToConvert,
        [NotNullWhen(true)] out OptionBehaviour? settingsOption,
        Transform? parent = null)
    {
        if (objToConvert is not ICustomSettingsOption<string>
            {
                CustomOption: ICustomStringOption customStringOption
            } ||
            !SettingsOptionPrototypeManager.Instance.TryCloneAndGet<StringOption>(out var stringOption, parent))
        {
            settingsOption = null;
            return false;
        }

        stringOption.Title = customStringOption.TitleName;
        stringOption.TitleText.text = customStringOption.Title;
        stringOption.Value = customStringOption.ValueInt;
        stringOption.Values = customStringOption.ValueNames;

        SettingsOptionConverterHelper.Bind(
            stringOption,
            customStringOption,
            () => customStringOption.ValueInt = stringOption.Value,
            () => stringOption.Value = customStringOption.ValueInt);

        settingsOption = stringOption;
        return true;
    }
}