using BepInEx;
using Mitochondria.Core.Api.Plugin;
using UnityEngine.SceneManagement;

namespace Mitochondria.Core.Api.Services;

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

    protected internal void OnLateUpdate()
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