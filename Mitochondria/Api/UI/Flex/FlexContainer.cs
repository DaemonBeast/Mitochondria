using System.Collections.Immutable;

namespace Mitochondria.Api.UI.Flex;

public abstract class FlexContainer<TFlex> : IFlexContainer<TFlex>
    where TFlex : IFlex
{
    public abstract bool CanAdd { get; }
    
    public abstract bool CanRemove { get; }

    public ImmutableArray<TFlex> Children => _children.ToImmutableArray();

    public ImmutableArray<TFlex> OrderedChildren => TryGetOrderedChildren().ToImmutableArray();

    public ImmutableArray<TFlex> ActiveOrderedChildren =>
        TryGetOrderedChildren().Where(f => f.Active).ToImmutableArray();

    public int Length => _children.Count;

    public  abstract bool CanBeOrdered { get; }

    public abstract bool CanChangeDirection { get; }
    
    public Direction Direction { get; private set; }

    public abstract bool CanChangeGap { get; }
    
    public float Gap { get; private set; }
    
    public abstract bool CanFlexAlign { get; }
    
    public Alignment FlexAlignment { get; private set; }
    
    public abstract bool CanCrossAlign { get; }
    
    public Alignment CrossAlignment { get; private set; }
    
    private readonly List<TFlex> _children;
    private IEnumerable<TFlex> _orderedChildren;
    private bool _isOrdered;

    protected FlexContainer(
        Direction direction = Direction.Down,
        float gap = 0f,
        Alignment flexAlignment = Alignment.Start,
        Alignment crossAlignment = Alignment.Start)
    {
        Direction = direction;
        Gap = gap;
        FlexAlignment = flexAlignment;
        CrossAlignment = crossAlignment;

        _children = new List<TFlex>();
        _orderedChildren = _children;
        _isOrdered = false;

        TrySetDirection(Direction);
        TrySetGap(Gap);
        TrySetFlexAlignment(FlexAlignment);
        TrySetCrossAlignment(CrossAlignment);
    }
    
    public bool TryAdd(TFlex child)
    {
        if (!CanAdd)
        {
            return false;
        }

        _children.Add(child);
        _isOrdered = false;

        child.OnOrderChanged += MarkUnordered;

        if (!OnAdd(child))
        {
            _children.Remove(child);
            child.OnOrderChanged -= MarkUnordered;
        }

        return true;
    }
    
    public bool TryAddRange(IEnumerable<TFlex> children)
    {
        var childrenArray = children.ToArray();
        
        if (!CanAdd)
        {
            return false;
        }
        
        _children.AddRange(childrenArray);
        _isOrdered = false;

        foreach (var child in childrenArray)
        {
            child.OnOrderChanged -= MarkUnordered;
            child.OnOrderChanged += MarkUnordered;
        }

        if (!OnAddRange(childrenArray))
        {
            foreach (var child in childrenArray)
            {
                _children.Remove(child);
                
                child.OnOrderChanged -= MarkUnordered;
            }
        }

        return true;
    }
    
    public bool TryRemove(TFlex child)
    {
        if (!CanRemove || !OnRemove(child))
        {
            return false;
        }
        
        _children.Remove(child);
        _isOrdered = false;

        child.OnOrderChanged -= MarkUnordered;

        return true;
    }

    public bool TrySetDirection(Direction direction)
    {
        if (!CanChangeDirection || !OnSetDirection(direction))
        {
            return false;
        }

        Direction = direction;

        return true;
    }

    public bool TrySetGap(float gap)
    {
        if (!CanChangeGap || !OnSetGap(gap))
        {
            return false;
        }

        Gap = gap;

        return true;
    }

    public bool TrySetFlexAlignment(Alignment alignment)
    {
        if (!CanFlexAlign || !OnSetFexAlignment(alignment))
        {
            return false;
        }

        FlexAlignment = alignment;

        return true;
    }

    public bool TrySetCrossAlignment(Alignment alignment)
    {
        if (!CanCrossAlign || !OnSetCrossAlignment(alignment))
        {
            return false;
        }

        CrossAlignment = alignment;

        return true;
    }
    
    protected virtual bool OnAdd(TFlex child) => true;

    protected virtual bool OnAddRange(IEnumerable<TFlex> children) => children.All(OnAdd);

    protected virtual bool OnRemove(TFlex child) => true;

    protected virtual bool OnSetDirection(Direction direction) => true;

    protected virtual bool OnSetGap(float gap) => true;

    protected virtual bool OnSetFexAlignment(Alignment alignment) => true;

    protected virtual bool OnSetCrossAlignment(Alignment alignment) => true;

    protected IEnumerable<TFlex> TryGetOrderedChildren()
    {
        if (!CanBeOrdered || _isOrdered)
        {
            return _orderedChildren;
        }
        
        _orderedChildren = _orderedChildren.OrderBy(f => f.Order);
        _isOrdered = true;

        return _orderedChildren;
    }
    
    private void MarkUnordered(int _)
        => _isOrdered = false;
}

public interface IFlexContainer<TFlex> : IFlexContainer
    where TFlex : IFlex
{
    public bool CanAdd { get; }
    
    public bool CanRemove { get; }
    
    public ImmutableArray<TFlex> Children { get; }
    
    public ImmutableArray<TFlex> OrderedChildren { get; }
    
    public ImmutableArray<TFlex> ActiveOrderedChildren { get; }
    
    public int Length { get; }

    public bool TryAdd(TFlex child);

    public bool TryAddRange(IEnumerable<TFlex> children);

    public bool TryRemove(TFlex child);
}

public interface IFlexContainer
{
    public bool CanBeOrdered { get; }
    
    public bool CanChangeDirection { get; }
    
    public Direction Direction { get; }

    public bool CanChangeGap { get; }
    
    public float Gap { get; }
    
    public bool CanFlexAlign { get; }
    
    public Alignment FlexAlignment { get; }
    
    public bool CanCrossAlign { get; }
    
    public Alignment CrossAlignment { get; }

    public bool TrySetDirection(Direction direction);

    public bool TrySetGap(float gap);

    public bool TrySetFlexAlignment(Alignment alignment);

    public bool TrySetCrossAlignment(Alignment alignment);
}