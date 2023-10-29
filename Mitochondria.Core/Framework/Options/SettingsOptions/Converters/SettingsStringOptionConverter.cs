using System.Diagnostics.CodeAnalysis;
using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Api.Options.SettingsOptions;
using Mitochondria.Core.Framework.Binding;
using Mitochondria.Core.Framework.Prototypes;

namespace Mitochondria.Core.Framework.Options.SettingsOptions.Converters;

[Converter]
public class SettingsStringOptionConverter : ISettingsOptionConverter<StringOption>
{
    public Type ConvertedType => typeof(StringOption);

    public bool ConvertsTo(Type convertedType)
        => convertedType.IsAssignableFrom(typeof(StringOption));

    public bool TryConvert(
        object objToConvert,
        [NotNullWhen(true)] out StringOption? convertedObj,
        TransformConvertArgs convertArgs)
    {
        if (objToConvert is not ICustomSettingsOption<string>
            {
                CustomOption: ICustomStringOption customStringOption
            } ||
            !PrototypeManager.Instance.TryCloneAndGet<StringOption>(out var stringOption, convertArgs.Parent))
        {
            convertedObj = null;
            return false;
        }

        stringOption.Title = customStringOption.TitleName;
        stringOption.TitleText.text = customStringOption.Title;
        stringOption.Value = customStringOption.ValueInt;
        stringOption.Values = customStringOption.ValueNames;

        convertedObj = stringOption;
        return true;
    }
}