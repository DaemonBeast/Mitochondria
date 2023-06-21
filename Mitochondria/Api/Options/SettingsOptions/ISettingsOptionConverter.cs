using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Mitochondria.Api.Options.SettingsOptions;

public interface ISettingsOptionConverter
{
    public bool CanConvert(Type typeToConvert);

    public bool ConvertsTo(Type convertedType);

    public bool TryConvert(
        object objToConvert,
        [NotNullWhen(true)] out OptionBehaviour? settingsOption,
        Transform? parent = null);
}