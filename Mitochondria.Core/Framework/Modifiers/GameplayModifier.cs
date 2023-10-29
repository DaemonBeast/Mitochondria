using InnerNet;
using Mitochondria.Core.Api.Modifiers;

namespace Mitochondria.Core.Framework.Modifiers;

public abstract class GameplayModifier : Modifier
{
    public override void Initialize()
    {
    }

    public virtual void OnLobbyJoined()
    {
    }

    public virtual void OnPlayerJoined(ClientData data)
    {
    }

    public virtual void OnPlayerLeft(ClientData data, DisconnectReasons reason)
    {
    }

    public virtual void OnBecomeHost()
    {
    }

    public virtual void OnIntroCutsceneStarted(IntroCutscene introCutscene)
    {
    }

    public virtual void OnTeamRevealed(IntroCutscene introCutscene)
    {
    }

    public virtual void OnRoleRevealed(IntroCutscene introCutscene)
    {
    }

    public virtual void OnIntroCutsceneEnding()
    {
    }

    public virtual void OnGameStarted(GameManager gameManager)
    {
    }

    /*public virtual void InitializeHud(HudManager hudManager)
    {
    }*/

    public virtual void OnGameEnded(GameManager gameManager)
    {
    }

    public virtual void OnDisconnect()
    {
    }
}