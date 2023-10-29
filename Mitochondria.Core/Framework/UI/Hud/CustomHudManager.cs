using Mitochondria.Core.Framework.UI.Hud.Containers;
using Mitochondria.Core.Framework.Utilities;

namespace Mitochondria.Core.Framework.UI.Hud;

public class CustomHudManager
{
    public static CustomHudManager Instance => Singleton<CustomHudManager>.Instance;

    public MainActionButtonsContainer MainActionButtonsContainer { get; }

    public CustomHudManager()
    {
        MainActionButtonsContainer = new MainActionButtonsContainer();
    }
}