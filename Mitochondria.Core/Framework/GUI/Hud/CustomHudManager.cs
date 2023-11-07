using Mitochondria.Core.Framework.GUI.Hud.Containers;
using Mitochondria.Core.Framework.Utilities;

namespace Mitochondria.Core.Framework.GUI.Hud;

public class CustomHudManager
{
    public static CustomHudManager Instance => Singleton<CustomHudManager>.Instance;

    public MainActionButtonsContainer MainActionButtonsContainer { get; }

    public CustomHudManager()
    {
        MainActionButtonsContainer = new MainActionButtonsContainer();
    }
}