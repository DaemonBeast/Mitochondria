using UnityEngine;

namespace Mitochondria.Api.UI.Flex;

public class Box : IEquatable<Box>
{
    public float? Top { get; }
    
    public float? Right { get; }
    
    public float? Bottom { get; }
    
    public float? Left { get; }

    public Box()
    {
        Top = Right = Bottom = Left = null;
    }

    public Box(float padding)
    {
        Top = Right = Bottom = Left = padding;
    }

    public Box(float? top = null, float? right = null, float? bottom = null, float? left = null)
    {
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
    }

    public Box CopyWithMask(Box toCopy, BooleanBox mask)
    {
        var top = mask.Top ? toCopy.Top : Top;
        var right = mask.Right ? toCopy.Right : Right;
        var bottom = mask.Bottom ? toCopy.Bottom : Bottom;
        var left = mask.Left ? toCopy.Left : Left;

        return new Box(top, right, bottom, left);
    }

    public bool Equals(Box? other)
    {
        return other != null &&
               (Top == null && other.Top == null || Mathf.Approximately(Top!.Value, other.Top!.Value)) &&
               (Right == null && other.Right == null || Mathf.Approximately(Right!.Value, other.Right!.Value)) &&
               (Bottom == null && other.Bottom == null || Mathf.Approximately(Bottom!.Value, other.Bottom!.Value)) &&
               (Left == null && other.Left == null || Mathf.Approximately(Left!.Value, other.Left!.Value));
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Box);
    }

    public override int GetHashCode()
    {
        return (Top, Right, Bottom, Left).GetHashCode();
    }
}