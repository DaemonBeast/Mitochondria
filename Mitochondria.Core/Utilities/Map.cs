using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Mitochondria.Core.Utilities;

public class Map<T1, T2> : IDictionary<T1, T2>
    where T1: notnull
    where T2 : notnull
{
    public Map<T2, T1> Reverse { get; }

    private readonly Dictionary<T1, T2> _forward;
    private readonly Dictionary<T2, T1> _back;

    public int Count => _forward.Count;

    public bool IsReadOnly => false;

    public ICollection<T1> Keys => _forward.Keys;

    public ICollection<T2> Values => _forward.Values;

    public Map()
    {
        _forward = new Dictionary<T1, T2>();
        _back = new Dictionary<T2, T1>();

        Reverse = new Map<T2, T1>(this, _back, _forward);
    }

    public Map(IEnumerable<KeyValuePair<T1, T2>> collection)
    {
        _forward = new Dictionary<T1, T2>(collection);
        _back = _forward.ToDictionary(p => p.Value, p => p.Key);

        Reverse = new Map<T2, T1>(this, _back, _forward);
    }

    private Map(Map<T2, T1> reverse, Dictionary<T1, T2> forward, Dictionary<T2, T1> back)
    {
        Reverse = reverse;

        _forward = forward;
        _back = back;
    }

    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        => _forward.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _forward.GetEnumerator();

    public void Add(KeyValuePair<T1, T2> item)
        => Add(item.Key, item.Value);

    public void Clear()
    {
        _forward.Clear();
        _back.Clear();
    }

    public bool Contains(KeyValuePair<T1, T2> item)
        // ReSharper disable once UsageOfDefaultStructEquality
        => _forward.Contains(item);

    public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        => ((ICollection<KeyValuePair<T1, T2>>) _forward).CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<T1, T2> item)
        => _forward.Remove(item.Key) && _back.Remove(item.Value);

    public void Add(T1 key, T2 value)
    {
        if (_forward.ContainsKey(key) || _back.ContainsKey(value))
        {
            throw new ArgumentException("Key or value already in map");
        }

        _forward.Add(key, value);
        _back.Add(value, key);
    }

    public bool ContainsKey(T1 key)
        => _forward.ContainsKey(key);

    public bool Remove(T1 key)
        => _forward.TryGetValue(key, out var value) && _forward.Remove(key) && _back.Remove(value);

    public bool TryGetValue(T1 key, [MaybeNullWhen(false)] out T2 value)
        => _forward.TryGetValue(key, out value);

    public T2 this[T1 key]
    {
        get => _forward[key];
        set
        {
            _forward[key] = value;
            _back[value] = key;
        }
    }
}
