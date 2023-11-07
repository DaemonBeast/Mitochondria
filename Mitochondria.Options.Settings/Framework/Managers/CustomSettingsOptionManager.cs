using System.Diagnostics.CodeAnalysis;
using AmongUs.GameOptions;
using Mitochondria.Core;
using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Api.Options;
using Mitochondria.Core.Framework.Binding;
using Mitochondria.Core.Framework.Configuration.Extensions;
using Mitochondria.Core.Framework.GUI.Extensions;
using Mitochondria.Core.Framework.Helpers;
using Mitochondria.Core.Framework.Utilities;
using Mitochondria.Core.Framework.Utilities.Extensions;
using Mitochondria.Options.Settings.Abstractions;
using Mitochondria.Options.Settings.Framework.Gui;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Options.Settings.Framework.Managers;

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

        using (var configHandle = customSettingsOption.GetOwner()!.GetConfig().Load<CustomSettingsOptionConfig>())
        {
            var customSettings = configHandle.Obj.CustomSettings;

            if (customSettings.TryGetValue(customOption.Id, out var value))
            {
                customOption.BoxedValue = value;
            }
            else
            {
                customSettings.Add(customOption.Id, customOption.BoxedValue);
            }
        }

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

        if (removeFromConfig)
        {
            using var configHandle = customSettingsOption.GetOwner()!.GetConfig().Load<CustomSettingsOptionConfig>();
            configHandle.Obj.CustomSettings.Remove(customSettingsOption.BoxedCustomOption.Id);
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
            gameObject.GetGuiElement<SettingsOptionElement>() is not { } element)
        {
            return;
        }

        if (customSettingsOption.Order != null)
        {
            element.Order = customSettingsOption.Order.Value;
        }

        var customOption = customSettingsOption.BoxedCustomOption;
        using var configHandle = customSettingsOption.GetOwner()!.GetConfig().Load<CustomSettingsOptionConfig>();
        configHandle.Obj.CustomSettings[customOption.Id] = customOption.BoxedValue;
    }

    private void TryCreateSettingsOptions(GameModes gameMode)
    {
        if (!TryGet(gameMode, out var customSettingsOptions))
        {
            return;
        }

        var settingsOptionContainer = SettingsOptionManager.Instance.Container;
        if (settingsOptionContainer == null)
        {
            return;
        }

        var convertArgs = new TransformConvertArgs(GameOptionsMenuHelper.Transform);

        foreach (var customSettingsOption in customSettingsOptions)
        {
            var customOption = customSettingsOption.BoxedCustomOption;

            if (_settingsOptions.TryGetValue(customSettingsOption, out var gameObject) && gameObject != null)
            {
                continue;
            }

            if (!ConverterManager.Instance.TryConvert<OptionBehaviour, TransformConvertArgs>(
                    customSettingsOption,
                    out var optionBehaviour,
                    convertArgs))
            {
                Logger<MitochondriaPlugin>.Warning($"Failed to create game object for \"{customOption.Title}\" option");
                
                continue;
            }

            gameObject = optionBehaviour.gameObject;

            if (!BindingManager.Instance.TryBind(optionBehaviour, customOption))
            {
                // Can't have a game object that's not in the container or there'll be overlapping issues
                gameObject.Destroy();

                Logger<MitochondriaPlugin>.Error($"\"{customOption.Title}\" does not have a corresponding gui element");

                continue;
            }

            var element = gameObject.SetGuiElement<SettingsOptionElement>();

            _settingsOptions[customSettingsOption] = gameObject;
            gameObject.name = customOption.Title.RemoveWhitespace();

            if (customSettingsOption.Order != null)
            {
                element.Order = customSettingsOption.Order.Value;
            }

            settingsOptionContainer.Add(element);
        }
    }
}