﻿using System.Collections.Immutable;
using Mitochondria.Api.Modifiers;
using Mitochondria.Framework.Utilities;

namespace Mitochondria.Framework.Modifiers;

public class ModifierManager
{
    public static ModifierManager Instance => Singleton<ModifierManager>.Instance;

    public ImmutableArray<IModifier> AllModifiers => _modifiers.Immutable;

    private readonly ImmutableArrayWrapper<IModifier> _modifiers;
    private readonly TypeGraph<IModifier> _typedModifiers;

    private ModifierManager()
    {
        _modifiers = new ImmutableArrayWrapper<IModifier>();
        _typedModifiers = new TypeGraph<IModifier>();
    }

    public void Add<TModifier>(TModifier modifier)
        where TModifier : IModifier
    {
        var modifierType = typeof(TModifier);

        _modifiers.Add(modifier);
        _typedModifiers.Add(modifierType, modifier);

        modifier.Initialize();
    }

    public bool Remove<TModifier>(TModifier modifier)
        where TModifier : IModifier
    {
        var result = _modifiers.Remove(modifier) &&
            _typedModifiers.TryGet(typeof(TModifier), out var modifiers) &&
            modifiers.Values.Remove(modifier);

        if (result)
        {
            modifier.Dispose();
        }

        return result;
    }

    public IEnumerable<TModifier> Get<TModifier>()
        where TModifier : class, IModifier
    {
        return _typedModifiers.TryGet(typeof(TModifier), out var modifiers)
            ? modifiers.Traverse().Cast<TModifier>().OrderBy(m => m.Priority)
            : ImmutableArray<TModifier>.Empty;
    }
}