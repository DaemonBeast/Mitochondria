using Mitochondria.Api.Services;
using Mitochondria.Framework.Modifiers;
using Mitochondria.Framework.Services;

namespace Mitochondria.Patches.Modifiers;

[Service]
public class GameplayModifierService : IService
{
    public void OnLobbyJoined()
    {
        var amHost = AmongUsClient.Instance.AmHost;

        foreach (var modifier in ModifierManager.Instance.Get<GameplayModifier>())
        {
            modifier.OnLobbyJoined();

            if (amHost)
            {
                modifier.OnBecomeHost();
            }
        }
    }

    /*public void OnHudStart(HudManager hudManager)
    {
        foreach (var modifier in ModifierManager.Instance.Get<GameplayModifier>())
        {
            modifier.InitializeHud(hudManager);
        }
    }*/
}