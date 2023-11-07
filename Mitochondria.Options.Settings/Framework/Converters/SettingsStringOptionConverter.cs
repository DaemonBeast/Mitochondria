using System.Diagnostics.CodeAnalysis;
using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Framework.Binding;
using Mitochondria.Core.Framework.Options;
using Mitochondria.Core.Framework.Prototypes;
using Mitochondria.Options.Settings.Abstractions;

namespace Mitochondria.Options.Settings.Framework.Converters;

[Converter]
public class SettingsStringOptionConverter : SettingsOptionConverter<StringOption>
{
    public override bool TryConvert(
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