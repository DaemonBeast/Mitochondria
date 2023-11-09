using System.Collections.Immutable;

namespace Mitochondria.Core.Framework.Utilities.DataStructures;

public class ImmutableDictionaryWrapper<TKey, TValue>
    where TKey : notnull
{
    public ImmutableDictionary<TKey, TValue> Immutable
    {
        get
        {
            if (_isDirty)
            {
                _isDirty = false;
                _immutableDictionary = Actual.ToImmutableDictionary();
            }

            return _immutableDictionary;
        }
    }

    public IDictionary<TKey, TValue> Actual { get; }

    private ImmutableDictionary<TKey, TValue> _immutableDictionary;
    private bool _isDirty;

    public ImmutableDictionaryWrapper(IDictionary<TKey, TValue>? actual = null)
    {
        Actual = actual ?? new Dictionary<TKey, TValue>();

        _immutableDictionary = ImmutableDictionary<TKey, TValue>.Empty;
        _isDirty = true;
    }

    public void Add(TKey key, TValue value)
    {
        Actual.Add(key, value);
        _isDirty = true;
    }

    public bool Remove(TKey key)
    {
        var removed = Actual.Remove(key);
        _isDirty = _isDirty || removed;
        return removed;
    }

    public void MarkDirty()
    {
        _isDirty = true;
    }
}