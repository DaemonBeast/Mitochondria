using Mitochondria.Core.Api.GUI;
using Mitochondria.Core.Framework.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Core.Framework.GUI.Extensions;

public static class GuiExtensions
{
    public static TGuiElement SetGuiElement<TGuiElement>(this GameObject gameObject)
        where TGuiElement : class, IGuiElement
    {
        if (Activator.CreateInstance(typeof(TGuiElement), true) is not TGuiElement guiElement)
        {
            throw new ArgumentException("Failed to create GUI element; make sure the constructor has no parameters");
        }

        var behaviour = gameObject.GetOrAddComponent<GuiElementBehaviour>();
        behaviour.Element = guiElement;
        ((GuiElement) behaviour.Element).GameObject = gameObject;

        return guiElement;
    }

    public static TGuiElement? GetGuiElement<TGuiElement>(this GameObject gameObject)
        where TGuiElement : class, IGuiElement
        => gameObject.GetGuiElement() as TGuiElement;

    public static IGuiElement? GetGuiElement(this GameObject gameObject)
        => gameObject.GetComponent<GuiElementBehaviour>().AsNullable()?.Element;

    public static TGuiElement GetOrSetGuiElement<TGuiElement>(this GameObject gameObject)
        where TGuiElement : class, IGuiElement
        => gameObject.GetGuiElement<TGuiElement>() ?? gameObject.SetGuiElement<TGuiElement>();

    public static TGuiContainer SetGuiContainer<TGuiContainer>(this GameObject gameObject)
        where TGuiContainer : class, IGuiContainer
    {
        if (Activator.CreateInstance(typeof(TGuiContainer), true) is not TGuiContainer guiContainer)
        {
            throw new ArgumentException("Failed to create GUI container; make sure the constructor has no parameters");
        }

        var behaviour = gameObject.GetOrAddComponent<GuiContainerBehaviour>();
        behaviour.Container = guiContainer;
        ((GuiContainer) behaviour.Container).GameObject = guiContainer.GameObject;

        return guiContainer;
    }

    public static TGuiContainer? GetGuiContainer<TGuiContainer>(this GameObject gameObject)
        where TGuiContainer : class, IGuiContainer
        => gameObject.GetGuiContainer() as TGuiContainer;

    public static IGuiContainer? GetGuiContainer(this GameObject gameObject)
        => gameObject.GetComponent<GuiContainerBehaviour>().AsNullable()?.Container;

    public static TGuiContainer GetOrSetGuiContainer<TGuiContainer>(this GameObject gameObject)
        where TGuiContainer : class, IGuiContainer
        => gameObject.GetGuiContainer<TGuiContainer>() ?? gameObject.SetGuiContainer<TGuiContainer>();
}