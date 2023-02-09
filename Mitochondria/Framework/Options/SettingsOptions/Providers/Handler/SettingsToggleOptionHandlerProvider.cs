using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options;
using Mitochondria.Api.Options.SettingsOptions;

namespace Mitochondria.Framework.Options.SettingsOptions.Providers.Handler;

[SettingsOptionHandlerProvider]
public class SettingsToggleOptionHandlerProvider : ISettingsOptionHandlerProvider
{
    public Type OptionType => typeof(ToggleOption);

    public bool Matches(Type customOptionType)
        => typeof(ICustomOption<bool>).IsAssignableFrom(customOptionType);

    public bool TryGetHandler(
        OptionBehaviour optionBehaviour,
        [NotNullWhen(true)] out Func<ICustomOption, bool>? handler)
    {
        var toggleOption = optionBehaviour.TryCast<ToggleOption>();
        if (toggleOption == null)
        {
            handler = null;
            return false;
        }

        handler = customOption =>
        {
            if (toggleOption == null || customOption is not ICustomOption<bool> customToggleOption)
            {
                return false;
            }

            toggleOption.CheckMark.enabled = customToggleOption.Value;

            return true;
        };

        return true;
    }
}