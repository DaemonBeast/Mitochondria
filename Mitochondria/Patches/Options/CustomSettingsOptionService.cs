using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AmongUs.GameOptions;
using Mitochondria.Api.Options;
using Mitochondria.Api.Options.SettingsOptions;
using Mitochondria.Api.Services;
using Mitochondria.Framework.Cache;
using Mitochondria.Framework.Options.SettingsOptions;
using Mitochondria.Framework.Plugin;
using Mitochondria.Framework.Services;
using Mitochondria.Framework.UI.Extensions;
using Mitochondria.Framework.UI.Flex.SettingsOptions;
using Mitochondria.Framework.Utilities.Extensions;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Patches.Options;

[Service]
public class CustomSettingsOptionService : IService
{
    void IService.OnStart()
    {
        AddOptionsWithAttribute();

        CustomSettingsOptionManager.Instance.OnCustomOptionAdded += CustomOptionAdded;
        CustomSettingsOptionManager.Instance.OnCustomOptionRemoved += CustomOptionRemoved;
        CustomSettingsOptionManager.Instance.OnCustomOptionChanged += CustomOptionChanged;

        SettingsOptionPatches.PostGameOptionsMenuOpened +=
            _ => TryCreateSettingsOptions(GameOptionsManager.Instance.currentGameMode);
    }

    private void AddOptionsWithAttribute()
    {
        var types = PluginManager.Instance.PluginInfos.Values
            .Select(p => p.Instance)
            .Where(o => o != null)
            .SelectMany(o => o.GetType().Assembly.GetTypes())
            .ToArray();
        
        var props = types
            .SelectMany(t => t.GetProperties())
            .Where(p => p.GetCustomAttributes<CustomSettingsOptionAttribute>().Any());

        foreach (var prop in props)
        {
            if (!prop.IsStatic())
            {
                Logger<MitochondriaPlugin>.Warning(
                    $"Property with {nameof(CustomSettingsOptionAttribute)} must be static");
            }
            
            var obj = prop.GetValue(null);
            foreach (var attribute in prop.GetCustomAttributes<CustomSettingsOptionAttribute>())
            {
                TryAddSettingsOption(obj, attribute);
            }
        }

        var fields = types
            .SelectMany(t => t.GetFields())
            .Where(f => f.GetCustomAttributes<CustomSettingsOptionAttribute>().Any());

        foreach (var field in fields)
        {
            if (!field.IsStatic)
            {
                Logger<MitochondriaPlugin>.Warning(
                    $"Field with {nameof(CustomSettingsOptionAttribute)} must be static");
            }
            
            var obj = field.GetValue(null);
            foreach (var attribute in field.GetCustomAttributes<CustomSettingsOptionAttribute>())
            {
                TryAddSettingsOption(obj, attribute);
            }
        }
    }

    private void TryAddSettingsOption(object? obj, CustomSettingsOptionAttribute attribute)
    {
        switch (obj)
        {
            case ICustomOption customOption:
            {
                var pluginType = customOption.GetOwner()!.Instance.GetType();
                var valueType = customOption.ValueType;
                
                var customSettingsOption = (ICustomSettingsOption) Activator.CreateInstance(
                    typeof(CustomSettingsOption<,>).MakeGenericType(pluginType, valueType),
                    customOption,
                    attribute.GameMode,
                    attribute.Order)!;

                CustomSettingsOptionManager.Instance.Add(customSettingsOption);
                    
                break;
            }
            case ICustomSettingsOption customSettingsOption:
            {
                CustomSettingsOptionManager.Instance.Add(customSettingsOption);
                    
                break;
            }
            default:
            {
                Logger<MitochondriaPlugin>.Warning(
                    $"Property with {nameof(CustomSettingsOptionAttribute)} must be a custom option");
                    
                break;
            }
        }
    }

    private void CustomOptionAdded(ICustomSettingsOption customSettingsOption)
    {
        if (GameOptionsManager.Instance.currentGameMode == customSettingsOption.GameMode)
        {
            TryCreateSettingsOptions(customSettingsOption.GameMode);
        }
    }
    
    private void CustomOptionRemoved(ICustomSettingsOption customSettingsOption)
    {
        var settingsOptions = SettingsOptionManager.Instance.SettingsOptions;
        
        if (GameOptionsManager.Instance.currentGameMode != customSettingsOption.GameMode ||
            !settingsOptions.ContainsKey(customSettingsOption))
        {
            return;
        }
        
        var gameObject = settingsOptions[customSettingsOption];
        if (gameObject != null)
        {
            gameObject.Destroy();
        }

        settingsOptions.Remove(customSettingsOption);
    }

    private void CustomOptionChanged(ICustomSettingsOption customSettingsOption)
    {
        if (GameOptionsManager.Instance.currentGameMode != customSettingsOption.GameMode ||
            !SettingsOptionManager.Instance.SettingsOptions.TryGetValue(customSettingsOption, out var gameObject) ||
            gameObject.AsFlex() is not SettingsOptionFlex flex)
        {
            return;
        }

        if (customSettingsOption.Order != null)
        {
            flex.TrySetOrder(customSettingsOption.Order.Value);
        }
    }

    private static bool TryCreateSettingsOption(
        ICustomSettingsOption customSettingsOption,
        [NotNullWhen(true)] out GameObject? gameObject)
    {
        var parent = MonoBehaviourProvider.TryGet(out GameOptionsMenu? gameOptionsMenu)
            ? gameOptionsMenu.transform
            : null;
        
        var factories = SettingsOptionManager.Instance.Factories;
        foreach (var converter in CustomSettingsOptionManager.Instance.Converters)
        {
            var matchingFactories = factories
                .Where(
                    f => f.ArgsType == converter.ReturnedConvertedType && f.ReturnedOptionType == converter.OptionType)
                .ToArray();

            if (matchingFactories.Length == 0 || !converter.TryConvert(customSettingsOption, out var args))
            {
                continue;
            }
            
            foreach (var factory in matchingFactories)
            {
                if (factory.UnsafeTryCreateOption(args, out gameObject, parent))
                {
                    return true;
                }
            }
        }

        gameObject = null;
        return false;
    }
    
    private static void AddHandler(ICustomOption customOption, OptionBehaviour optionBehaviour)
    {
        var customOptionType = customOption.GetType();
        
        foreach (var handlerProvider in SettingsOptionManager.Instance.HandlerProviders)
        {
            if (!handlerProvider.Matches(customOptionType) ||
                !handlerProvider.TryGetHandler(optionBehaviour, out var handler))
            {
                continue;
            }

            void Handler(ICustomOption _)
            {
                if (handler.Invoke(customOption))
                {
                    optionBehaviour.OnValueChanged.Invoke(optionBehaviour);
                }
                else
                {
                    customOption.OnChanged -= Handler;
                }
            }

            customOption.OnChanged += Handler;

            break;
        }
    }

    private void TryCreateSettingsOptions(GameModes gameMode)
    {
        if (!CustomSettingsOptionManager.Instance.CustomSettingsOptions
                .TryGetValue(gameMode, out var customSettingsOptions))
        {
            return;
        }
        
        var settingsOptions = SettingsOptionManager.Instance.SettingsOptions;
        
        foreach (var customSettingsOption in customSettingsOptions)
        {
            var customOption = customSettingsOption.BoxedCustomOption;
            
            if (settingsOptions.TryGetValue(customSettingsOption, out var gameObject) && gameObject != null ||
                !TryCreateSettingsOption(customSettingsOption, out gameObject))
            {
                Logger<MitochondriaPlugin>.Warning($"Failed to create game object for \"{customOption.Title}\" option");
                
                continue;
            }
            
            if (gameObject.AsFlex() is not SettingsOptionFlex flex)
            {
                // Can't have a game object that's not in the flex container or there'll be overlapping issues
                gameObject.Destroy();

                Logger<MitochondriaPlugin>.Error($"\"{customOption.Title}\" is not flexible");

                continue;
            }

            settingsOptions[customSettingsOption] = gameObject;
            
            AddHandler(customOption, gameObject.GetComponent<OptionBehaviour>());
            
            gameObject.name = customOption.Title;

            if (customSettingsOption.Order != null)
            {
                flex.TrySetOrder(customSettingsOption.Order.Value);
            }

            SettingsOptionManager.Instance.FlexContainer.TryAdd(flex);
        }
    }
}