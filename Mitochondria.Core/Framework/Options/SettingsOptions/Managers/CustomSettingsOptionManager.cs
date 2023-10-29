using System.Diagnostics.CodeAnalysis;
using AmongUs.GameOptions;
using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Api.Options;
using Mitochondria.Core.Api.Options.SettingsOptions;
using Mitochondria.Core.Framework.Binding;
using Mitochondria.Core.Framework.Configuration;
using Mitochondria.Core.Framework.Helpers;
using Mitochondria.Core.Framework.UI.Extensions;
using Mitochondria.Core.Framework.UI.Flex.SettingsOptions;
using Mitochondria.Core.Framework.Utilities;
using Mitochondria.Core.Framework.Utilities.Extensions;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Core.Framework.Options.SettingsOptions.Managers;

public class CustomSettingsOptionManager
{
    public static CustomSettingsOptionManager Instance => Singleton<CustomSettingsOptionManager>.Instance;

    public IEnumerable<ICustomSettingsOption> CustomSettingsOptions => _customSettingsOptions.SelectMany(e => e.Value);

    private readonly Dictionary<GameModes, List<ICustomSettingsOption>> _customSettingsOptions;

    private readonly Dictionary<ICustomSettingsOption, GameObject> _settingsOptions;

    private CustomSettingsOptionManager()
    {
        _customSettingsOptions = new Dictionary<GameModes, List<ICustomSettingsOption>>();
        _settingsOptions = new Dictionary<ICustomSettingsOption, GameObject>();

        GameOptionsMenuHelper.OnAfterOpened += () =>
        {
            if (GameOptionsManager.Instance != null)
            {
                TryCreateSettingsOptions(GameOptionsManager.Instance.currentGameMode);
            }
        };
    }

    public void AddRange(IEnumerable<ICustomSettingsOption> customSettingsOptions)
    {
        foreach (var customSettingsOption in customSettingsOptions)
        {
            Add(customSettingsOption);
        }
    }

    public void Add(ICustomSettingsOption customSettingsOption)
    {
        var customOption = customSettingsOption.BoxedCustomOption;
        
        if (_customSettingsOptions.TryGetValue(customSettingsOption.GameMode, out var customSettingsOptions))
        {
            if (customSettingsOptions.Any(c => c.BoxedCustomOption == customOption))
            {
                Logger<MitochondriaPlugin>.Warning("Added custom settings option for same gamemode multiple times");
            }
        }
        else
        {
            customSettingsOptions = new List<ICustomSettingsOption>();
            _customSettingsOptions.Add(customSettingsOption.GameMode, customSettingsOptions);
        }
        
        customSettingsOption.OnChanged += CustomSettingsOptionChanged;

        customSettingsOptions.Add(customSettingsOption);

        var config = Config.Instance.Of(customSettingsOption.GetOwner()!.Instance.GetType());
        var customSettingsOptionsConfig = config.Load<CustomSettingsOptionsConfig>();
        var customSettings = customSettingsOptionsConfig.CustomSettings;

        if (customSettings.TryGetValue(customOption.Id, out var value))
        {
            customOption.BoxedValue = value;
        }
        else
        {
            customSettings.Add(customOption.Id, customOption.BoxedValue);
        }

        config.Save(customSettingsOptionsConfig);

        if (GameOptionsMenuHelper.Current != null &&
            GameOptionsManager.Instance != null &&
            GameOptionsManager.Instance.currentGameMode == customSettingsOption.GameMode)
        {
            TryCreateSettingsOptions(customSettingsOption.GameMode);
        }
    }

    public void RemoveRange(IEnumerable<ICustomSettingsOption> customSettingsOptions, bool removeFromConfig = true)
    {
        foreach (var customSettingsOption in customSettingsOptions)
        {
            Remove(customSettingsOption, removeFromConfig);
        }
    }

    public void Remove(ICustomSettingsOption customSettingsOption, bool removeFromConfig = true)
    {
        var customSettingsOptions = _customSettingsOptions.GetValueOrDefault(customSettingsOption.GameMode);
        if (customSettingsOptions == null || !customSettingsOptions.Remove(customSettingsOption))
        {
            return;
        }

        customSettingsOption.OnChanged -= CustomSettingsOptionChanged;

        // why did I include this???
        /*if (GameOptionsManager.Instance?.currentGameMode != customSettingsOption.GameMode ||
            !_settingsOptions.ContainsKey(customSettingsOption))
        {
            return;
        }*/

        if (removeFromConfig)
        {
            var config = Config.Instance.Of(customSettingsOption.GetOwner()!.Instance.GetType());
            var customSettingsOptionsConfig = config.Load<CustomSettingsOptionsConfig>();
            customSettingsOptionsConfig.CustomSettings.Remove(customSettingsOption.BoxedCustomOption.Id);
            config.Save(customSettingsOptionsConfig);
        }

        if (_settingsOptions.Remove(customSettingsOption, out var gameObject) && gameObject != null)
        {
            gameObject.Destroy();
        }
    }

    public IEnumerable<ICustomSettingsOption> Find(ICustomOption customOption)
    {
        foreach (var customSettingsOptions in _customSettingsOptions.Values)
        {
            foreach (var customSettingsOption in customSettingsOptions)
            {
                if (customSettingsOption.BoxedCustomOption == customOption)
                {
                    yield return customSettingsOption;

                    break;
                }
            }
        }
    }

    public bool TryGet(
        GameModes gameMode,
        [NotNullWhen(true)] out ICustomSettingsOption[]? customSettingsOptions)
    {
        var result = _customSettingsOptions.TryGetValue(gameMode, out var c);
        customSettingsOptions = c?.ToArray();

        return result;
    }

    public bool TryGet(
        GameModes gameMode,
        ICustomOption customOption,
        [NotNullWhen(true)] out ICustomSettingsOption? customSettingsOption)
        => (customSettingsOption = _customSettingsOptions
            .GetValueOrDefault(gameMode)
            ?.FirstOrDefault(c => c.BoxedCustomOption == customOption)) != null;

    public void ResetAllOptions(GameModes gameMode)
    {
        if (!_customSettingsOptions.TryGetValue(gameMode, out var customSettingsOptions))
        {
            return;
        }
        
        foreach (var customSettingsOption in customSettingsOptions)
        {
            customSettingsOption.BoxedCustomOption.ResetValue();
        }
    }

    public bool TryGetSettingsOption(
        GameObject gameObject,
        [NotNullWhen(true)] out ICustomSettingsOption? settingsOption)
        => (settingsOption =
            _settingsOptions.FirstOrDefault(e => e.Value.IsEqualTo(gameObject)).DefaultToNull()?.Key) != null;

    private void CustomSettingsOptionChanged(ICustomSettingsOption customSettingsOption)
    {
        if (GameOptionsManager.Instance?.currentGameMode != customSettingsOption.GameMode ||
            !_settingsOptions.TryGetValue(customSettingsOption, out var gameObject) ||
            gameObject.AsFlex() is not SettingsOptionFlex flex)
        {
            return;
        }

        if (customSettingsOption.Order != null)
        {
            flex.TrySetOrder(customSettingsOption.Order.Value);
        }

        var customOption = customSettingsOption.BoxedCustomOption;
        var config = Config.Instance.Of(customSettingsOption.GetOwner()!.Instance.GetType());
        var customSettingsOptionsConfig = config.Load<CustomSettingsOptionsConfig>();
        customSettingsOptionsConfig.CustomSettings[customOption.Id] = customOption.BoxedValue;
        config.Save(customSettingsOptionsConfig);
    }

    private void TryCreateSettingsOptions(GameModes gameMode)
    {
        if (!TryGet(gameMode, out var customSettingsOptions))
        {
            return;
        }

        var convertArgs = new TransformConvertArgs(GameOptionsMenuHelper.Transform);
        var flexContainer = SettingsOptionManager.Instance.FlexContainer;
        
        foreach (var customSettingsOption in customSettingsOptions)
        {
            var customOption = customSettingsOption.BoxedCustomOption;

            if (_settingsOptions.TryGetValue(customSettingsOption, out var gameObject) && gameObject != null)
            {
                continue;
            }

            if (!Converter.Instance.TryConvert<OptionBehaviour, TransformConvertArgs>(
                    customSettingsOption,
                    out var optionBehaviour,
                    convertArgs))
            {
                Logger<MitochondriaPlugin>.Warning($"Failed to create game object for \"{customOption.Title}\" option");
                
                continue;
            }

            gameObject = optionBehaviour.gameObject;
            
            if (!Binder.Instance.TryBind(optionBehaviour, customOption) ||
                gameObject.AsFlex() is not SettingsOptionFlex flex)
            {
                // Can't have a game object that's not in the flex container or there'll be overlapping issues
                gameObject.Destroy();

                Logger<MitochondriaPlugin>.Error($"\"{customOption.Title}\" is not flexible");

                continue;
            }

            _settingsOptions[customSettingsOption] = gameObject;

            gameObject.name = customOption.Title.RemoveWhitespace();

            if (customSettingsOption.Order != null)
            {
                flex.TrySetOrder(customSettingsOption.Order.Value);
            }

            flexContainer.TryAdd(flex);
        }
    }
}