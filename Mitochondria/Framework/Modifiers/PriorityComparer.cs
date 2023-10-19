using Mitochondria.Api.Modifiers;

namespace Mitochondria.Framework.Modifiers;

public class PriorityComparer<TModifier> : Comparer<TModifier>
    where TModifier : IModifier
{
    public override int Compare(TModifier? x, TModifier? y)
        => x == null || y == null ? 0 : x.Priority - y.Priority;
}