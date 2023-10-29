namespace Mitochondria.Core.Api.UI.Flex;

public readonly struct BooleanBox
{
    public static BooleanBox All { get; }
    
    public static BooleanBox None { get; }
    
    public bool Top { get; }
    
    public bool Right { get; }
    
    public bool Bottom { get; }
    
    public bool Left { get; }

    static BooleanBox()
    {
        All = new BooleanBox(true);
        None = new BooleanBox(false);
    }

    public BooleanBox(bool value)
    {
        Top = Right = Bottom = Left = value;
    }

    public BooleanBox(bool top, bool right, bool bottom, bool left)
    {
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
    }

    public bool Any()
    {
        return Top || Right || Bottom || Left;
    }
}