namespace Mitochondria.Api.UI.Flex;

public abstract class Flex : IFlex
{
    public abstract BooleanBox CanBePadded { get; }
    
    public Box Padding { get; private set; }

    public int Order { get; private set; }

    public abstract bool Active { get; }

    public abstract bool CanCrossAlign { get; }
    
    public Alignment CrossAlignment { get; private set; }

    public event IFlex.OrderChangedHandler? OnOrderChanged;

    protected Flex(Box? padding = null, int order = 0, Alignment crossAlignment = Alignment.Start)
    {
        Padding = padding ?? new Box(0f);
        Order = order;
        CrossAlignment = crossAlignment;

        TrySetPadding(Padding);
        TrySetOrder(Order);
        TrySetCrossAlignment(CrossAlignment);
    }

    public bool TrySetPadding(Box padding)
    {
        if (!CanBePadded.Any())
        {
            return true;
        }
        
        var newPadding = Padding.CopyWithMask(padding, CanBePadded);
        if (!OnSetPadding(newPadding))
        {
            return false;
        }

        Padding = newPadding;
        
        return true;
    }
    
    public bool TrySetOrder(int order)
    {
        if (!OnSetOrder(order))
        {
            return false;
        }

        Order = order;
        OnOrderChanged?.Invoke(order);

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

    protected virtual bool OnSetPadding(Box padding) => true;

    protected virtual bool OnSetOrder(int order) => true;

    protected virtual bool OnSetCrossAlignment(Alignment alignment) => true;
}

public interface IFlex
{
    public BooleanBox CanBePadded { get; }
    
    public Box Padding { get; }

    public int Order { get; }

    public bool Active { get; }
    
    public bool CanCrossAlign { get; }
    
    public Alignment CrossAlignment { get; }
    
    public delegate void OrderChangedHandler(int order);

    public event OrderChangedHandler? OnOrderChanged;

    public bool TrySetPadding(Box padding);

    public bool TrySetOrder(int order);

    public bool TrySetCrossAlignment(Alignment alignment);
}