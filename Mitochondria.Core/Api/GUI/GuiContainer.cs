using System.Collections.Immutable;
using Mitochondria.Core.Framework.Utilities.DataStructures;
using UnityEngine;

namespace Mitochondria.Core.Api.GUI;

public class GuiContainer<TGuiElement> : GuiContainer
    where TGuiElement : IGuiElement
{
    public ImmutableArray<TGuiElement> Children => ChildrenWrapper.Immutable;

    protected readonly ImmutableArrayWrapper<TGuiElement> ChildrenWrapper;

    protected GuiContainer()
    {
        ChildrenWrapper = new ImmutableArrayWrapper<TGuiElement>();
    }

    public void Add(TGuiElement guiElement)
    {
        if (ChildrenWrapper.Actual.Contains(guiElement))
        {
            return;
        }

        if (guiElement.Container is { } container)
        {
            container.Remove(guiElement);
        }

        ChildrenWrapper.Add(guiElement);
        OnAdd(guiElement);
    }

    public void AddRange(IEnumerable<TGuiElement> guiElements)
    {
        foreach (var guiElement in guiElements)
        {
            Add(guiElement);
        }
    }

    public sealed override bool Remove(IGuiElement guiElement)
    {
        if (guiElement is not TGuiElement element)
        {
            return false;
        }

        var result = ChildrenWrapper.Remove(element);

        if (result)
        {
            ((GuiElement) guiElement).Container = null;
            OnRemove(element);
        }

        return result;
    }

    public sealed override void RemoveAll(IEnumerable<IGuiElement> guiElements)
    {
        foreach (var guiElement in guiElements)
        {
            Remove(guiElement);
        }
    }

    protected virtual void OnAdd(TGuiElement guiElement)
    {
    }

    protected virtual void OnRemove(TGuiElement guiElement)
    {
    }
}

public abstract class GuiContainer : IGuiContainer
{
    public GameObject? GameObject { get; internal set; }

    public abstract bool Remove(IGuiElement guiElement);

    public abstract void RemoveAll(IEnumerable<IGuiElement> guiElements);
}

public interface IGuiContainer
{
    public GameObject? GameObject { get; }

    public bool Remove(IGuiElement guiElement);

    public void RemoveAll(IEnumerable<IGuiElement> guiElements);
}