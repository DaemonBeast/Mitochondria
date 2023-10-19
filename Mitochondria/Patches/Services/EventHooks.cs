using HarmonyLib;
using Mitochondria.Api.Services;
using Mitochondria.Framework.Services;
using Reactor.Utilities.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mitochondria.Patches.Services;

[RegisterInIl2Cpp]
public class EventHooks : MonoBehaviour
{
    private EventHooks()
    {
        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>) ((scene, loadMode) =>
        {
            ActionManager<Scene, LoadSceneMode>.Instance.Invoke(nameof(IService.OnSceneLoaded), scene, loadMode);
        }));
    }

    private void Start()
        => ActionManager.Instance.Invoke(nameof(IService.OnStart));

    private void Update()
        => ActionManager.Instance.Invoke(nameof(IService.OnUpdate));

    private void OnApplicationQuit()
        => ActionManager.Instance.Invoke(nameof(IService.OnGracefulExit));

    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    public static class GameJoinedPatch
    {
        public static void Postfix()
        {
            ActionManager.Instance.Invoke(nameof(IService.OnLobbyJoined));
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class HudManagerStartPatch
    {
        public static void Postfix(HudManager __instance)
        {
            ActionManager<HudManager>.Instance.Invoke(nameof(IService.OnHudStart), __instance);
        }
    }
}