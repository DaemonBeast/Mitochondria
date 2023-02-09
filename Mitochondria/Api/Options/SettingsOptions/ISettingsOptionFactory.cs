using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Mitochondria.Api.Options.SettingsOptions;

public interface ISettingsOptionFactory<in TArgs> : ISettingsOptionFactory
{
    public bool TryCreateOption(TArgs args, [NotNullWhen(true)] out GameObject? gameObject, Transform? parent = null);
}

public interface ISettingsOptionFactory
{
    public Type ReturnedOptionType { get; }
    
    public Type ArgsType { get; }
    
    public bool UnsafeTryCreateOption(
        object? args,
        [NotNullWhen(true)] out GameObject? gameObject,
        Transform? parent = null);
}