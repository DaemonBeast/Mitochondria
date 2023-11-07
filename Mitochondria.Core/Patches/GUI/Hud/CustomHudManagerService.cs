using Mitochondria.Core.Api.Services;
using Mitochondria.Core.Framework.GUI.Hud;
using Mitochondria.Core.Framework.Services;

namespace Mitochondria.Core.Patches.GUI.Hud;

[Service]
public class CustomHudManagerService : IService
{
    public void OnUpdate()
    {
        CustomHudManager.Instance.MainActionButtonsContainer.CreateActionButtons();
    }
}