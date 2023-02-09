using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Mitochondria.Api.Options.SettingsOptions;

public abstract class SettingsOptionFactory<TArgs> : ISettingsOptionFactory<TArgs>
{
    public abstract Type ReturnedOptionType { get; }

    public abstract Type ArgsType { get; }

    public abstract bool TryCreateOption(
        TArgs args,
        [NotNullWhen(true)] out GameObject? gameObject,
        Transform? parent = null);

    public bool UnsafeTryCreateOption(
        object? args,
        [NotNullWhen(true)] out GameObject? gameObject,
        Transform? parent = null)
    {
        gameObject = null;
        return args is TArgs tArgs && TryCreateOption(tArgs, out gameObject, parent);
    }
}