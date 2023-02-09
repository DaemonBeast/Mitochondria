using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Mitochondria.Api.Options.SettingsOptions;

public interface ISettingsOptionProvider
{
    public Type ReturnedOptionType { get; }
    
    public bool TryGetNewOption([NotNullWhen(true)] out GameObject? gameObject, Transform? parent = null);
}