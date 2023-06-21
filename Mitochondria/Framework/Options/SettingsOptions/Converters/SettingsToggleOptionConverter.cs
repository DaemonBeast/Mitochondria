using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options.SettingsOptions;
using UnityEngine;

namespace Mitochondria.Framework.Options.SettingsOptions.Converters;

[SettingsOptionConverter]
public class SettingsToggleOptionConverter : ISettingsOptionConverter
{
    public bool CanConvert(Type typeToConvert)
        => typeof(ICustomSettingsOption<bool>).IsAssignableFrom(typeToConvert);

    public bool ConvertsTo(Type convertedType)
        => convertedType.IsAssignableFrom(typeof(ToggleOption));

    public bool TryConvert(
        object objToConvert,
        [NotNullWhen(true)] out OptionBehaviour? settingsOption,
        Transform? parent = null)
    {
        if (objToConvert is not ICustomSettingsOption<bool> { CustomOption: { } customToggleOption } ||
            !SettingsOptionPrototypeManager.Instance.TryCloneAndGet<ToggleOption>(out var toggleOption, parent))
        {
            settingsOption = null;
            return false;
        }

        toggleOption.Title = customToggleOption.TitleName;
        toggleOption.TitleText.text = customToggleOption.Title;
        toggleOption.CheckMark.enabled = customToggleOption.Value;
        
        SettingsOptionConverterHelper.Bind(
            toggleOption,
            customToggleOption,
            () => customToggleOption.Value = toggleOption.CheckMark.enabled,
            () => toggleOption.CheckMark.enabled = customToggleOption.Value);

        settingsOption = toggleOption;
        return true;
    }
}