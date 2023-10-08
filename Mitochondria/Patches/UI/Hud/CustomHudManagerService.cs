using Mitochondria.Api.Services;
using Mitochondria.Framework.Services;
using Mitochondria.Framework.UI.Hud;

namespace Mitochondria.Patches.UI.Hud;

[Service]
public class CustomHudManagerService : IService
{
    void IService.OnUpdate()
    {
        CustomHudManager.Instance.MainActionButtonsContainer.CreateActionButtons();
    }
}