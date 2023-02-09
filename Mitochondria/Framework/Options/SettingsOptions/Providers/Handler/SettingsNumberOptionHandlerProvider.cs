using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options;
using Mitochondria.Api.Options.SettingsOptions;

namespace Mitochondria.Framework.Options.SettingsOptions.Providers.Handler;

[SettingsOptionHandlerProvider]
public class SettingsNumberOptionHandlerProvider : ISettingsOptionHandlerProvider
{
    public Type OptionType => typeof(NumberOption);

    public bool Matches(Type customOptionType)
        => typeof(ICustomNumberOption).IsAssignableFrom(customOptionType);

    public bool TryGetHandler(
        OptionBehaviour optionBehaviour,
        [NotNullWhen(true)] out Func<ICustomOption, bool>? handler)
    {
        var numberOption = optionBehaviour.TryCast<NumberOption>();
        if (numberOption == null)
        {
            handler = null;
            return false;
        }

        handler = customOption =>
        {
            if (numberOption == null || customOption is not ICustomNumberOption customNumberOption)
            {
                return false;
            }

            numberOption.Value = customNumberOption.Value;

            return true;
        };

        return true;
    }
}