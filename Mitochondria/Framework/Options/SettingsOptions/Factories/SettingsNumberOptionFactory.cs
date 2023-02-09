using System.Diagnostics.CodeAnalysis;
using Il2CppInterop.Runtime;
using Mitochondria.Api.Options.SettingsOptions;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Framework.Options.SettingsOptions.Factories;

[SettingsOptionFactory]
public class SettingsNumberOptionFactory : SettingsOptionFactory<NumberOptionArgs>
{
    public override Type ReturnedOptionType => typeof(NumberOption);

    public override Type ArgsType => typeof(NumberOptionArgs);

    public override bool TryCreateOption(
        NumberOptionArgs args,
        [NotNullWhen(true)] out GameObject? gameObject,
        Transform? parent = null)
    {
        if (!SettingsOptionManager.Instance.TryGetNewOption(ReturnedOptionType, out gameObject, parent))
        {
            return false;
        }

        if (!gameObject.TryGetComponent(Il2CppType.Of<NumberOption>(), out var component))
        {
            gameObject.Destroy();

            return false;
        }

        var numberOption = component.Cast<NumberOption>();

        numberOption.Title = args.Title;
        numberOption.TitleText.text = TranslationController.Instance.GetString(args.Title);
        numberOption.Value = args.Value;
        numberOption.ValidRange = args.Range;
        numberOption.Increment = args.Step;
        numberOption.ZeroIsInfinity = args.ZeroIsInfinity;

        SettingsOptionFactoryHelper.AddHandler(numberOption, args.ValueChangedHandler);

        return true;
    }
}