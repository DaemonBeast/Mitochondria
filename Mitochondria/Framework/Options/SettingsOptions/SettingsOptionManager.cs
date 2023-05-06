using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Options.SettingsOptions;
using Mitochondria.Framework.UI.Flex.SettingsOptions;
using Mitochondria.Framework.Utilities;
using Mitochondria.Framework.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Framework.Options.SettingsOptions;

public class SettingsOptionManager
{
    public static SettingsOptionManager Instance => Singleton<SettingsOptionManager>.Instance;

    public ImmutableArray<ISettingsOptionProvider> Providers => _providers.ToImmutableArray();

    public ImmutableArray<ISettingsOptionFactory> Factories => _factories.ToImmutableArray();

    public ImmutableArray<ISettingsOptionHandlerProvider> HandlerProviders => _handlerProviders.ToImmutableArray();

    public Dictionary<ICustomSettingsOption, GameObject> SettingsOptions { get; }

    public SettingsOptionFlexContainer FlexContainer { get; }

    private readonly List<ISettingsOptionProvider> _providers;
    private readonly List<ISettingsOptionFactory> _factories;
    private readonly List<ISettingsOptionHandlerProvider> _handlerProviders;

    private SettingsOptionManager()
    {
        SettingsOptions = new Dictionary<ICustomSettingsOption, GameObject>();
        FlexContainer = new SettingsOptionFlexContainer();
        
        _providers = new List<ISettingsOptionProvider>();
        _factories = new List<ISettingsOptionFactory>();
        _handlerProviders = new List<ISettingsOptionHandlerProvider>();
    }

    public void RegisterProvider(ISettingsOptionProvider settingsOptionProvider)
    {
        if (!_providers.Contains(settingsOptionProvider))
        {
            _providers.Add(settingsOptionProvider);
        }
    }

    public void RegisterProviders(IEnumerable<ISettingsOptionProvider> settingsOptionProviders)
    {
        foreach (var provider in settingsOptionProviders)
        {
            RegisterProvider(provider);
        }
    }

    public void UnregisterProvider(ISettingsOptionProvider settingsOptionProvider)
        => _providers.Remove(settingsOptionProvider);

    public void RegisterFactory(ISettingsOptionFactory settingsOptionFactory)
    {
        if (!_factories.Contains(settingsOptionFactory))
        {
            _factories.Add(settingsOptionFactory);
        }
    }

    public void RegisterFactories(IEnumerable<ISettingsOptionFactory> settingsOptionFactories)
    {
        foreach (var factory in settingsOptionFactories)
        {
            RegisterFactory(factory);
        }
    }

    public void UnregisterFactory(ISettingsOptionFactory settingsOptionFactory)
        => _factories.Remove(settingsOptionFactory);

    public void RegisterHandlerProvider(ISettingsOptionHandlerProvider settingsOptionHandlerProvider)
    {
        if (!_handlerProviders.Contains(settingsOptionHandlerProvider))
        {
            _handlerProviders.Add(settingsOptionHandlerProvider);
        }
    }

    public void RegisterHandlerProviders(IEnumerable<ISettingsOptionHandlerProvider> settingsOptionHandlerProviders)
    {
        foreach (var handlerProvider in settingsOptionHandlerProviders)
        {
            RegisterHandlerProvider(handlerProvider);
        }
    }

    public void UnregisterHandlerProvider(ISettingsOptionHandlerProvider settingsOptionHandlerProvider)
        => _handlerProviders.Remove(settingsOptionHandlerProvider);

    public bool TryGetProvider<TSettingsOptionProvider>(
        [NotNullWhen(true)] out TSettingsOptionProvider? settingsOptionProvider)
        where TSettingsOptionProvider : class, ISettingsOptionProvider
    {
        return (settingsOptionProvider = _providers.FirstOrDefault(s => s is TSettingsOptionProvider)
            as TSettingsOptionProvider) != null;
    }

    public bool TryGetFactory<TSettingsOptionFactory>(
        [NotNullWhen(true)] out TSettingsOptionFactory? settingsOptionFactory)
        where TSettingsOptionFactory : class, ISettingsOptionFactory
    {
        return (settingsOptionFactory = _factories.FirstOrDefault(s => s is TSettingsOptionFactory)
            as TSettingsOptionFactory) != null;
    }

    public bool TryGetHandlerProvider<TSettingsHandlerProvider>(
        [NotNullWhen(true)] out TSettingsHandlerProvider? settingsHandlerProvider)
        where TSettingsHandlerProvider : class, ISettingsOptionHandlerProvider
    {
        return (settingsHandlerProvider = _handlerProviders.FirstOrDefault(s => s is TSettingsHandlerProvider)
            as TSettingsHandlerProvider) != null;
    }

    public bool TryGetSettingsOption(
        GameObject gameObject,
        [NotNullWhen(true)] out ICustomSettingsOption? settingsOption)
    {
        settingsOption = SettingsOptions.FirstOrDefault(e => e.Value.IsEqualTo(gameObject)).AsNullable()?.Key;

        return settingsOption != null;
    }

    public bool TryGetNewOption(
        Type optionType,
        [NotNullWhen(true)] out GameObject? gameObject,
        Transform? parent = null)
    {
        foreach (var provider in _providers)
        {
            if (provider.ReturnedOptionType == optionType && provider.TryGetNewOption(out gameObject, parent))
            {
                return true;
            }
        }

        gameObject = null;
        return false;
    }

    public bool TryCreateOption(
        object args,
        [NotNullWhen(true)] out GameObject? gameObject,
        Transform? parent = null,
        Type? returnedOptionType = null)
    {
        var argsType = args.GetType();
        
        foreach (var factory in _factories)
        {
            if ((returnedOptionType == null || factory.ReturnedOptionType == returnedOptionType) &&
                factory.ArgsType == argsType &&
                factory.UnsafeTryCreateOption(args, out gameObject, parent))
            {
                return true;
            }
        }

        gameObject = null;
        return false;
    }
}