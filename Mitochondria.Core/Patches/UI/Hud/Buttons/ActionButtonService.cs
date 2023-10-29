using Mitochondria.Core.Api.Services;
using Mitochondria.Core.Api.UI.Hud.Buttons;
using Mitochondria.Core.Framework.Services;
using Mitochondria.Core.Framework.UI.Hud;

namespace Mitochondria.Core.Patches.UI.Hud.Buttons;

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