using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using AmongUs.GameOptions;
using Il2CppInterop.Generator.Extensions;
using Mitochondria.Api.Options;
using Mitochondria.Api.Options.SettingsOptions;
using Mitochondria.Framework.Networking;
using Mitochondria.Framework.Utilities;
using Reactor.Utilities;

namespace Mitochondria.Framework.Options.SettingsOptions;

public class CustomSettingsOptionManager
{
    public static CustomSettingsOptionManager Instance => Singleton<CustomSettingsOptionManager>.Instance;

    public ImmutableDictionary<GameModes, ImmutableArray<ICustomSettingsOption>> CustomSettingsOptions =>
        _customSettingsOptions.ToImmutableDictionary(e => e.Key, e => e.Value.ToImmutableArray());

    public ImmutableArray<ICustomSettingsOptionConverter> Converters => _converters.ToImmutableArray();

    public delegate void CustomSettingsOptionAddedHandler(ICustomSettingsOption customSettingsOption);

    public delegate void CustomSettingsOptionRemovedHandler(ICustomSettingsOption customSettingsOption);

    public delegate void CustomSettingsOptionChangedHandler(ICustomSettingsOption customSettingsOption);

    public delegate void CustomOptionChangedHandler(ICustomOption customOption);

    public event Action? OnCustomOptionCollectionChanged;
    public event CustomSettingsOptionAddedHandler? OnCustomSettingsOptionAdded;
    public event CustomSettingsOptionRemovedHandler? OnCustomSettingsOptionRemoved;
    public event CustomSettingsOptionChangedHandler? OnCustomSettingsOptionChanged;
    public event CustomOptionChangedHandler? OnCustomOptionChanged;

    private readonly Dictionary<GameModes, List<ICustomSettingsOption>> _customSettingsOptions;
    private readonly List<ICustomSettingsOptionConverter> _converters;

    private CustomSettingsOptionManager()
    {
        _customSettingsOptions = new Dictionary<GameModes, List<ICustomSettingsOption>>();
        _converters = new List<ICustomSettingsOptionConverter>();

        OnCustomSettingsOptionAdded += _ => OnCustomOptionCollectionChanged?.Invoke();
        OnCustomSettingsOptionRemoved += _ => OnCustomOptionCollectionChanged?.Invoke();
    }

    public void Add(ICustomSettingsOption customSettingsOption)
    {
        var customOption = customSettingsOption.BoxedCustomOption;
        
        var customSettingsOptions = _customSettingsOptions
            .GetOrCreate(customSettingsOption.GameMode, _ => new List<ICustomSettingsOption>());

        if (customSettingsOptions.Any(c => c.BoxedCustomOption == customOption))
        {
            Logger<MitochondriaPlugin>.Warning("Custom settings option added for same gamemode multiple times");

            return;
        }

        if (!Find(customOption).Any())
        {
            customOption.OnChanged += CustomOptionChanged;
        }
        
        customSettingsOptions.Add(customSettingsOption);

        OnCustomSettingsOptionAdded?.Invoke(customSettingsOption);
    }

    public void Remove(ICustomSettingsOption customSettingsOption)
    {
        var customSettingsOptions = _customSettingsOptions.GetValueOrDefault(customSettingsOption.GameMode);
        if (customSettingsOptions == null || !customSettingsOptions.Remove(customSettingsOption))
        {
            // uhhhhhh, I mean, I'm not complaining, but this shouldn't happen
            return;
        }

        var customOption = customSettingsOption.BoxedCustomOption;

        if (!Find(customOption).Any())
        {
            customOption.OnChanged -= CustomOptionChanged;
        }

        OnCustomSettingsOptionRemoved?.Invoke(customSettingsOption);
    }

    public IEnumerable<ICustomSettingsOption> Find(ICustomOption customOption)
    {
        foreach (var gameMode in Enum.GetValues<GameModes>())
        {
            if (TryGet(gameMode, customOption, out var customSettingsOption))
            {
                yield return customSettingsOption;
            }
        }
    }

    public bool TryGet(
        GameModes gameMode,
        ICustomOption customOption,
        [NotNullWhen(true)] out ICustomSettingsOption? customSettingsOption)
    {
        if (CustomSettingsOptions.TryGetValue(gameMode, out var customSettingsOptions))
        {
            return (customSettingsOption =
                customSettingsOptions.FirstOrDefault(c => c.BoxedCustomOption == customOption)) != null;
        }

        customSettingsOption = null;
        return false;
    }

    public void RegisterConverter(ICustomSettingsOptionConverter customSettingsOptionConverter)
    {
        if (!_converters.Contains(customSettingsOptionConverter))
        {
            _converters.Add(customSettingsOptionConverter);
        }
    }

    public void RegisterConverters(IEnumerable<ICustomSettingsOptionConverter> customSettingsOptionConverters)
    {
        foreach (var converter in customSettingsOptionConverters)
        {
            RegisterConverter(converter);
        }
    }

    public void UnregisterConverter(ICustomSettingsOptionConverter customSettingsOptionConverter)
        => _converters.Remove(customSettingsOptionConverter);
    
    public bool TryGetConverter<TCustomSettingsOptionConverter>(
        [NotNullWhen(true)] out TCustomSettingsOptionConverter? customSettingsOptionConverter)
        where TCustomSettingsOptionConverter : class, ICustomSettingsOptionConverter
    {
        return (customSettingsOptionConverter = _converters.FirstOrDefault(c => c is TCustomSettingsOptionConverter)
            as TCustomSettingsOptionConverter) != null;
    }
    
    public void ResetAllOptions(GameModes gameMode)
    {
        var customSettingsOptions = _customSettingsOptions.GetValueOrDefault(gameMode);
        if (customSettingsOptions == null)
        {
            return;
        }

        foreach (var customSettingsOption in customSettingsOptions)
        {
            customSettingsOption.BoxedCustomOption.ResetValue();
        }
    }

    public bool TryConvertCustomOption(
        ICustomSettingsOption customSettingsOption,
        [NotNullWhen(true)] out object? args,
        Type? returnedConvertedType = null,
        Type? optionType = null)
    {
        var customOptionType = customSettingsOption.BoxedCustomOption.GetType();

        foreach (var converter in _converters)
        {
            if ((returnedConvertedType == null || converter.ReturnedConvertedType == returnedConvertedType) &&
                (optionType == null || converter.OptionType == optionType) &&
                converter.Matches(customOptionType) &&
                converter.TryConvert(customSettingsOption, out args))
            {
                return true;
            }
        }

        args = null;
        return false;
    }

    private void CustomOptionChanged(ICustomOption customOption)
    {
        if (customOption.Sync)
        {
            SyncableManager.Instance.Sync(customOption);
        }

        OnCustomOptionChanged?.Invoke(customOption);

        foreach (var customSettingsOption in Find(customOption))
        {
            OnCustomSettingsOptionChanged?.Invoke(customSettingsOption);
        }
    }
}