using BepInEx;
using Mitochondria.Api.Owner;
using UnityEngine.SceneManagement;

namespace Mitochondria.Api.Services;

public interface IService : IOwned
{
    protected internal void OnPluginLoaded(PluginInfo pluginInfo)
    {
    }

    protected internal void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
    }

    protected internal void OnStart()
    {
    }

    protected internal void OnUpdate()
    {
    }

    protected internal void OnGracefulExit()
    {
    }

    protected internal void OnLobbyJoined()
    {
    }

    protected internal void OnHudStart(HudManager hudManager)
    {
    }
}