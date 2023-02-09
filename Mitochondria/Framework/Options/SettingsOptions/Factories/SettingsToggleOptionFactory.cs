using System.Diagnostics.CodeAnalysis;
using Il2CppInterop.Runtime;
using Mitochondria.Api.Options.SettingsOptions;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Framework.Options.SettingsOptions.Factories;

[SettingsOptionFactory]
public class SettingsToggleOptionFactory : SettingsOptionFactory<ToggleOptionArgs>
{
    public override Type ReturnedOptionType => typeof(ToggleOption);

    public override Type ArgsType => typeof(ToggleOptionArgs);

    public override bool TryCreateOption(
        ToggleOptionArgs args,
        [NotNullWhen(true)] out GameObject? gameObject,
        Transform? parent = null)
    {
        if (!SettingsOptionManager.Instance.TryGetNewOption(ReturnedOptionType, out gameObject, parent))
        {
            return false;
        }

        if (!gameObject.TryGetComponent(Il2CppType.Of<ToggleOption>(), out var component))
        {
            gameObject.Destroy();
            
            return false;
        }

        var toggleOption = component.Cast<ToggleOption>();

        toggleOption.Title = args.Title;
        toggleOption.TitleText.text = TranslationController.Instance.GetString(args.Title);
        toggleOption.CheckMark.enabled = args.Value;
        
        SettingsOptionFactoryHelper.AddHandler(toggleOption, args.ValueChangedHandler);
        
        return true;
    }
}