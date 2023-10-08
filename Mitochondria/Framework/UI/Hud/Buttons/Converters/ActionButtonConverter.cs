using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Binding;
using Mitochondria.Api.UI.Hud.Buttons;
using Mitochondria.Framework.Binding;
using Mitochondria.Framework.Prototypes;
using Mitochondria.Framework.Utilities.Extensions;
using Reactor.Utilities.Extensions;

namespace Mitochondria.Framework.UI.Hud.Buttons.Converters;

[Converter]
public class ActionButtonConverter : IActionButtonConverter
{
    public Type ConvertedType => typeof(ActionButton);

    public bool ConvertsTo(Type convertedType)
        => convertedType.IsAssignableFrom(typeof(ActionButton));

    public bool TryConvert(
        object objToConvert,
        [NotNullWhen(true)] out ActionButton? convertedObj,
        TransformConvertArgs convertArgs)
    {
        if (objToConvert is not ICustomActionButton customActionButton ||
            !PrototypeManager.Instance.TryCloneAndGet<AbilityButton>(out var useButton, convertArgs.Parent))
        {
            // TODO: account for ability button `commsDown` field
            convertedObj = null;
            return false;
        }

        useButton.gameObject.name = $"{customActionButton.Title.RemoveWhitespace()}Button";

        useButton.buttonLabelText.GetComponent<TextTranslatorTMP>().Destroy();
        useButton.buttonLabelText.text = customActionButton.Title;

        useButton.graphic.sprite = customActionButton.Icon.Load();
        useButton.graphic.SetCooldownNormalizedUvs();

        convertedObj = useButton;
        return true;
    }
}

public interface IActionButtonConverter : IConverter<ActionButton, TransformConvertArgs>
{
}