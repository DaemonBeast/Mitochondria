using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options;
using Mitochondria.Api.Options.SettingsOptions;

namespace Mitochondria.Framework.Options.SettingsOptions.Providers.Handler;

[SettingsOptionHandlerProvider]
public class SettingsStringOptionHandlerProvider : ISettingsOptionHandlerProvider
{
    public Type OptionType => typeof(StringOption);

    public bool Matches(Type customOptionType)
        => typeof(ICustomStringOption).IsAssignableFrom(customOptionType);

    public bool TryGetHandler(
        OptionBehaviour optionBehaviour,
        [NotNullWhen(true)] out Func<ICustomOption, bool>? handler)
    {
        var stringOption = optionBehaviour.TryCast<StringOption>();
        if (stringOption == null)
        {
            handler = null;
            return false;
        }

        handler = customOption =>
        {
            if (stringOption == null || customOption is not ICustomStringOption customStringOption)
            {
                return false;
            }

            stringOption.Value = customStringOption.ValueInt;

            return true;
        };

        return true;
    }
}