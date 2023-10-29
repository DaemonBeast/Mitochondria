using Mitochondria.Core.Api.Services;
using Mitochondria.Core.Framework.Services;
using Mitochondria.Core.Framework.UI.Hud;

namespace Mitochondria.Core.Patches.UI.Hud;

[Service]
public class CustomHudManagerService : IService
{
    public void OnUpdate()
    {
        CustomHudManager.Instance.MainActionButtonsContainer.CreateActionButtons();
    }
}