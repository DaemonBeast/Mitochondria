using AmongUs.Data;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.GameModes;

[RegisterInIl2Cpp]
public class CustomGameModeBehaviour : MonoBehaviour
{
    public ICustomGameMode CustomGameMode { get; }

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

        CustomGameMode.Initialize();
        _initialized = true;
    }

    private void OnDestroy()
    {
        if (!_initialized) return;

        CustomGameMode.Deinitialize();
    }
}
