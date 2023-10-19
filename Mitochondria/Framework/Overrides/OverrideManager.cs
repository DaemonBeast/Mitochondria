using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Mitochondria.Api.Overrides;
using Mitochondria.Framework.Utilities;

namespace Mitochondria.Framework.Overrides;

public class OverrideManager
{
    public static OverrideManager Instance => Singleton<OverrideManager>.Instance;

    public ImmutableDictionary<Type, ImmutableList<IOverride>> Overrides => _overrides.ToImmutableDictionary();

    public ImmutableDictionary<Type, IOverride> MergedOverrides => _mergedOverrides.ToImmutableDictionary();

    private readonly Dictionary<Type, ImmutableList<IOverride>> _overrides;
    private readonly Dictionary<Type, IOverride> _mergedOverrides;

    private OverrideManager()
    {
        _overrides = new Dictionary<Type, ImmutableList<IOverride>>();
        _mergedOverrides = new Dictionary<Type, IOverride>();
    }

    public void Add<TOverride>(TOverride @override)
        where TOverride : IOverride<TOverride>
    {
        var overrideType = typeof(TOverride);

        var existingOverrides = _overrides.GetValueOrDefault(overrideType) ?? Enumerable.Empty<IOverride>();
        var orderedOverrides = existingOverrides.AddItem(@override).OrderBy(o => o.Priority).ToArray();
        
        _overrides[overrideType] = orderedOverrides.ToImmutableList();
        
        UpdateMergedOverride<TOverride>();
    }

    public bool TryGetMergedOverride<TOverride>([NotNullWhen(true)] out TOverride? @override)
        where TOverride : class, IOverride<TOverride>
    {
        var result = _mergedOverrides.TryGetValue(typeof(TOverride), out var o);

        @override = o as TOverride;
        return result;
    }

    public void Remove<TOverride>(TOverride? @override)
        where TOverride : IOverride<TOverride>
    {
        if (@override == null)
        {
            return;
        }
        
        var overrideType = typeof(TOverride);

        if (!_overrides.TryGetValue(overrideType, out var overrides) || !overrides.Contains(@override))
        {
            return;
        }

        _overrides[overrideType] = overrides.Remove(@override);
        
        UpdateMergedOverride<TOverride>();
    }

    private void UpdateMergedOverride<TOverride>()
        where TOverride : IOverride<TOverride>
    {
        var overrideType = typeof(TOverride);

        var overrides = _overrides[overrideType];
        if (overrides.Count == 0)
        {
            return;
        }

        _mergedOverrides[overrideType] = overrides.Cast<TOverride>().Aggregate((merged, next) => next.Override(merged));
    }
}