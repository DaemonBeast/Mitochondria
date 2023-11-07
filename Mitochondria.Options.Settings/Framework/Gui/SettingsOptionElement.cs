using Mitochondria.Core.Api.GUI;

namespace Mitochondria.Options.Settings.Framework.Gui;

public class SettingsOptionElement : GuiElement, IOrderableElement
{
    public int Order { get; set; }

    public SettingsOptionElement()
    {
        Order = -1;
    }
}