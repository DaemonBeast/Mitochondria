using System.Collections.Immutable;
using Mitochondria.Framework.Utilities.Extensions;

namespace Mitochondria.Api.UI.Hud;

public abstract class HudContainer<THudElement> : IHudContainer<THudElement>
    where THudElement : IHudElement
{
    public ImmutableArray<THudElement> Children => _children.Immutable;

    private readonly ImmutableArrayWrapper<THudElement> _children;

    protected HudContainer()
    {
        _children = new ImmutableArrayWrapper<THudElement>();
    }

    public void Add(THudElement hudElement)
    {
        _children.Add(hudElement);

        OnAdd(hudElement);
    }

    public void Remove(THudElement hudElement)
    {
        _children.Remove(hudElement);

        OnRemove(hudElement);
    }

    protected virtual void OnAdd(THudElement hudElement)
    {
    }

    protected virtual void OnRemove(THudElement hudElement)
    {
    }
}

public interface IHudContainer<THudElement>
    where THudElement : IHudElement
{
    public ImmutableArray<THudElement> Children { get; }

    public void Add(THudElement hudElement);

    public void Remove(THudElement hudElement);
}