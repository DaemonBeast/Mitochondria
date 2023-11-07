namespace Mitochondria.Core.Api.GUI;

public interface IOrderableElement : IGuiElement
{
    public int Order { get; }
}