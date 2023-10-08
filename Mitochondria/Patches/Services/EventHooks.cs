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
            ServiceManager.Instance.TryInvokeMethod(nameof(IService.OnSceneLoaded), scene, loadMode);
        }));
    }

    private void Start()
        => ServiceManager.Instance.TryInvokeMethod(nameof(IService.OnStart));

    private void Update()
        => ServiceManager.Instance.TryInvokeMethod(nameof(IService.OnUpdate));

    private void OnApplicationQuit()
        => ServiceManager.Instance.TryInvokeMethod(nameof(IService.OnGracefulExit));

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
    public static class GameJoinedPatch
    {
        public static void Postfix()
        {
            ServiceManager.Instance.TryInvokeMethod(nameof(IService.OnLobbyJoined));
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class HudManagerStartPatch
    {
        public static void Postfix(HudManager __instance)
        {
            ServiceManager.Instance.TryInvokeMethod(nameof(IService.OnHudStart), __instance);
        }
    }
}