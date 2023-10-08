using System.Collections.Immutable;

namespace Mitochondria.Framework.Utilities.Extensions;

public class ImmutableArrayWrapper<T>
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

    public List<T> Actual { get; }

    private ImmutableArray<T> _immutableArray;
    private bool _isDirty;

    public ImmutableArrayWrapper(IEnumerable<T>? original = null)
    {
        Actual = original?.ToList() ?? new List<T>();

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