using System.Diagnostics.CodeAnalysis;
using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Framework.Binding;
using Mitochondria.Core.Framework.Options;
using Mitochondria.Core.Framework.Prototypes;
using Mitochondria.Options.Settings.Abstractions;

namespace Mitochondria.Options.Settings.Framework.Converters;

[Converter]
public class SettingsNumberOptionConverter : SettingsOptionConverter<NumberOption>
{
    public override bool TryConvert(
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