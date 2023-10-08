using Mitochondria.Framework.UI.Hud.Containers;
using Mitochondria.Framework.Utilities;

namespace Mitochondria.Framework.UI.Hud;

public class CustomHudManager
{
    public static CustomHudManager Instance => Singleton<CustomHudManager>.Instance;

    public MainActionButtonsContainer MainActionButtonsContainer { get; }

    public CustomHudManager()
    {
        MainActionButtonsContainer = new MainActionButtonsContainer();
    }
}