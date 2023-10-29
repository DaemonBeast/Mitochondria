using System.Diagnostics.CodeAnalysis;
using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Api.Options.SettingsOptions;
using Mitochondria.Core.Framework.Binding;
using Mitochondria.Core.Framework.Prototypes;

namespace Mitochondria.Core.Framework.Options.SettingsOptions.Converters;

[Converter]
public class SettingsNumberOptionConverter : ISettingsOptionConverter<NumberOption>
{
    public Type ConvertedType => typeof(NumberOption);

    public bool ConvertsTo(Type convertedType)
        => convertedType.IsAssignableFrom(typeof(NumberOption));

    public bool TryConvert(
        object objToConvert,
        [NotNullWhen(true)] out NumberOption? convertedObj,
        TransformConvertArgs convertArgs)
    {
        if (objToConvert is not ICustomSettingsOption<float> { CustomOption: ICustomNumberOption customNumberOption } ||
            !PrototypeManager.Instance.TryCloneAndGet<NumberOption>(out var numberOption, convertArgs.Parent))
        {
            convertedObj = null;
            return false;
        }

        numberOption.Title = customNumberOption.TitleName;
        numberOption.TitleText.text = customNumberOption.Title;
        numberOption.Value = customNumberOption.Value;
        numberOption.ValidRange = customNumberOption.Range;
        numberOption.Increment = customNumberOption.Step;
        numberOption.ZeroIsInfinity = customNumberOption.ZeroIsInfinity;

        convertedObj = numberOption;
        return true;
    }
}