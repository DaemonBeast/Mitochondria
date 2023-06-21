using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options.SettingsOptions;
using Mitochondria.Framework.Helpers;
using Mitochondria.Framework.Utilities;
using Reactor.Utilities;

namespace Mitochondria.Framework.Options.SettingsOptions;

public class SettingsOptionConverterManager
{
    public static SettingsOptionConverterManager Instance => Singleton<SettingsOptionConverterManager>.Instance;

    private readonly Dictionary<Type, ISettingsOptionConverter> _converters;

    private SettingsOptionConverterManager()
    {
        _converters = new Dictionary<Type, ISettingsOptionConverter>();
    }

    public void Register(Type settingsOptionConverterType)
    {
        if (!typeof(ISettingsOptionConverter).IsAssignableFrom(settingsOptionConverterType))
        {
            Logger<MitochondriaPlugin>.Error(
                $"{settingsOptionConverterType} is not of type {typeof(ISettingsOptionConverter)}");

            return;
        }

        if (Activator.CreateInstance(settingsOptionConverterType, true)
            is not ISettingsOptionConverter settingsOptionConverter)
        {
            Logger<MitochondriaPlugin>.Error($"Failed to create instance of {settingsOptionConverterType}");
            return;
        }

        _converters[settingsOptionConverterType] = settingsOptionConverter;
    }

    public bool TryUnregister(Type settingsOptionConverterType)
        => _converters.Remove(settingsOptionConverterType);

    public bool TryConvert(
        object objToConvert,
        [NotNullWhen(true)] out OptionBehaviour? settingsOption,
        Type? convertedType = null)
    {
        foreach (var converter in _converters.Values)
        {
            if ((convertedType == null || converter.ConvertsTo(convertedType)) &&
                converter.TryConvert(objToConvert, out settingsOption, GameOptionsMenuHelper.Transform))
            {
                return true;
            }
        }

        settingsOption = null;
        return false;
    }
}