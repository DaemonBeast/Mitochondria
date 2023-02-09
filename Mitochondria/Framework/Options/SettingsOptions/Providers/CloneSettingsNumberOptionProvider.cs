using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options.SettingsOptions;
using UnityEngine;

namespace Mitochondria.Framework.Options.SettingsOptions.Providers;

[SettingsOptionProvider]
public class CloneSettingsNumberOptionProvider : ISettingsOptionProvider
{
    public Type ReturnedOptionType => typeof(NumberOption);

    public bool TryGetNewOption([NotNullWhen(true)] out GameObject? gameObject, Transform? parent = null)
        => CloneSettingsOptionProviderHelper.TryGetNewOption(
            CloneSettingsOptionProviderHelper.NumberOptionTemplate, out gameObject, parent);
}