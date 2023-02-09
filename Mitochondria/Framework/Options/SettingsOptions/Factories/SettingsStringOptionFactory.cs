using System.Diagnostics.CodeAnalysis;
using Il2CppInterop.Runtime;
using Mitochondria.Api.Options.SettingsOptions;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Framework.Options.SettingsOptions.Factories;

[SettingsOptionFactory]
public class SettingsStringOptionFactory : SettingsOptionFactory<StringOptionArgs>
{
    public override Type ReturnedOptionType => typeof(StringOption);

    public override Type ArgsType => typeof(StringOptionArgs);

    public override bool TryCreateOption(
        StringOptionArgs args,
        [NotNullWhen(true)] out GameObject? gameObject,
        Transform? parent = null)
    {
        if (!SettingsOptionManager.Instance.TryGetNewOption(ReturnedOptionType, out gameObject, parent))
        {
            return false;
        }

        if (!gameObject.TryGetComponent(Il2CppType.Of<StringOption>(), out var component))
        {
            gameObject.Destroy();

            return false;
        }

        var stringOption = component.Cast<StringOption>();

        stringOption.Title = args.Title;
        stringOption.TitleText.text = TranslationController.Instance.GetString(args.Title);
        stringOption.Value = args.Value;
        stringOption.Values = args.Values.ToArray();
        
        SettingsOptionFactoryHelper.AddHandler(stringOption, args.ValueChangedHandler);

        return true;
    }
}