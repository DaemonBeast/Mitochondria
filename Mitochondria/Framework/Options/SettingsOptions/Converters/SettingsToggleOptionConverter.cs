using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Binding;
using Mitochondria.Api.Options.SettingsOptions;
using Mitochondria.Framework.Binding;
using Mitochondria.Framework.Prototypes;

namespace Mitochondria.Framework.Options.SettingsOptions.Converters;

[Converter]
public class SettingsToggleOptionConverter : ISettingsOptionConverter<ToggleOption>
{
    public Type ConvertedType => typeof(ToggleOption);

    public bool ConvertsTo(Type convertedType)
        => convertedType.IsAssignableFrom(typeof(ToggleOption));

    public bool TryConvert(
        object objToConvert,
        [NotNullWhen(true)] out ToggleOption? convertedObj,
        TransformConvertArgs convertArgs)
    {
        if (objToConvert is not ICustomSettingsOption<bool> { CustomOption: { } customToggleOption } ||
            !PrototypeManager.Instance.TryCloneAndGet<ToggleOption>(out var toggleOption, convertArgs.Parent))
        {
            convertedObj = null;
            return false;
        }

        toggleOption.Title = customToggleOption.TitleName;
        toggleOption.TitleText.text = customToggleOption.Title;
        toggleOption.CheckMark.enabled = customToggleOption.Value;

        convertedObj = toggleOption;
        return true;
    }
}