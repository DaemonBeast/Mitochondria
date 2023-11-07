using UnityEngine;

namespace Mitochondria.Core.Api.GUI;

public abstract class GuiElement : IGuiElement
{
    public IGuiContainer? Container { get; internal set; }

    public GameObject? GameObject { get; internal set; }
}

public interface IGuiElement
{
    public IGuiContainer? Container { get; }

    public GameObject? GameObject { get; }
}