using AmongUs.Data;
using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.GameModes;

[RegisterInIl2Cpp]
public class CustomGameModeBehaviour : MonoBehaviour
{
    [HideFromIl2Cpp]
    public BaseCustomGameMode CustomGameMode { get; }

    private bool _initialized;

    public CustomGameModeBehaviour(IntPtr ptr) : base(ptr)
    {
        CustomGameModeManager.GameModes.TryGetValue(
            DataManager.Settings.Multiplayer.LastPlayedGameMode,
            out var customGameMode);

        CustomGameMode = customGameMode!;
    }

    private void Awake()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (CustomGameMode == null)
        {
            this.Destroy();
            return;
        }

        CustomGameMode.Flow = CustomGameMode.CreateFlow();
        _initialized = true;
    }

    private void OnDestroy()
    {
        if (!_initialized) return;

        CustomGameMode.Flow?.Dispose();
    }
}
