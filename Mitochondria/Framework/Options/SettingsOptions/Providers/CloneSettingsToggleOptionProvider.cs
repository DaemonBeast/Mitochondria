using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options.SettingsOptions;
using UnityEngine;

namespace Mitochondria.Framework.Options.SettingsOptions.Providers;

[SettingsOptionProvider]
public class CloneSettingsToggleOptionProvider : ISettingsOptionProvider
{
    public Type ReturnedOptionType => typeof(ToggleOption);

    public bool TryGetNewOption([NotNullWhen(true)] out GameObject? gameObject, Transform? parent = null)
        => CloneSettingsOptionProviderHelper.TryGetNewOption(
            CloneSettingsOptionProviderHelper.ToggleOptionTemplate, out gameObject, parent);
}