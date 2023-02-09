using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options.SettingsOptions;
using UnityEngine;

namespace Mitochondria.Framework.Options.SettingsOptions.Providers;

[SettingsOptionProvider]
public class CloneSettingsStringOptionProvider : ISettingsOptionProvider
{
    public Type ReturnedOptionType => typeof(StringOption);

    public bool TryGetNewOption([NotNullWhen(true)] out GameObject? gameObject, Transform? parent = null)
        => CloneSettingsOptionProviderHelper.TryGetNewOption(
            CloneSettingsOptionProviderHelper.StringOptionTemplate, out gameObject, parent);
}