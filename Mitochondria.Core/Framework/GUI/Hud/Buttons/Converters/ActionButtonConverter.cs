using System.Diagnostics.CodeAnalysis;
using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Api.GUI.Hud.Buttons;
using Mitochondria.Core.Framework.Binding;
using Mitochondria.Core.Framework.Prototypes;
using Mitochondria.Core.Framework.Utilities.Extensions;
using Reactor.Utilities.Extensions;

namespace Mitochondria.Core.Framework.GUI.Hud.Buttons.Converters;

[Converter]
public class ActionButtonConverter : Api.Binding.Converter<ActionButton, TransformConvertArgs>
{
    public override bool TryConvert(
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