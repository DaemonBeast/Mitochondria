using System.Collections.Immutable;

namespace Mitochondria.Framework.Utilities;

public class ImmutableArrayWrapper<T> : ImmutableArrayWrapper
{
    public ImmutableArray<T> Immutable
    {
        get
        {
            if (_isDirty)
            {
                _isDirty = false;
                _immutableArray = Actual.ToImmutableArray();
            }

            return _immutableArray;
        }
    }

    public ICollection<T> Actual { get; }

    private ImmutableArray<T> _immutableArray;
    private bool _isDirty;

    public ImmutableArrayWrapper(ICollection<T>? actual = null)
    {
        Actual = actual ?? new List<T>();

        _immutableArray = ImmutableArray<T>.Empty;
        _isDirty = true;
    }

    public void Add(T element)
    {
        Actual.Add(element);
        _isDirty = true;
    }

    public bool Remove(T element)
    {
        var removed = Actual.Remove(element);
        _isDirty = _isDirty || removed;
        return removed;
    }

    public void MarkDirty()
    {
        _isDirty = true;
    }
}

public class ImmutableArrayWrapper
{
}