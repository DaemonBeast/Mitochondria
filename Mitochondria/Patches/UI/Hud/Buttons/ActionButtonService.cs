using Mitochondria.Api.Services;
using Mitochondria.Api.UI.Hud.Buttons;
using Mitochondria.Framework.Services;
using Mitochondria.Framework.UI.Hud;

namespace Mitochondria.Patches.UI.Hud.Buttons;

[Service]
public class ActionButtonService : IService
{
    void IService.OnUpdate()
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