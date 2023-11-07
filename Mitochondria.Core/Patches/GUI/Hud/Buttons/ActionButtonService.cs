using Mitochondria.Core.Api.GUI.Hud.Buttons;
using Mitochondria.Core.Api.Services;
using Mitochondria.Core.Framework.GUI.Hud;
using Mitochondria.Core.Framework.Services;

namespace Mitochondria.Core.Patches.GUI.Hud.Buttons;

[Service]
public class ActionButtonService : IService
{
    public void OnUpdate()
    {
        foreach (var button in CustomHudManager.Instance.MainActionButtonsContainer.Children)
        {
            if (button is CustomActionButton customActionButton)
            {
                customActionButton.AdvanceCooldown();
            }
        }
    }
}