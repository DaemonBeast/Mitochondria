using System.Diagnostics.CodeAnalysis;

namespace Mitochondria.Api.Options.SettingsOptions;

public interface ISettingsOptionHandlerProvider
{
    public Type OptionType { get; }

    public bool Matches(Type customOptionType);

    public bool TryGetHandler(
        OptionBehaviour optionBehaviour,
        [NotNullWhen(true)] out Func<ICustomOption, bool>? handler);
}