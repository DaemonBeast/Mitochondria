using System.Diagnostics.CodeAnalysis;

namespace Mitochondria.GameModes.Utilities.Extensions;

public static class GameManagerExtensions
{
    public static bool TryGetActiveCustomGameMode(
        this GameManager gameManager,
        [NotNullWhen(true)] out BaseCustomGameMode? customGameMode)
    {
        if (gameManager.TryGetComponent<CustomGameModeBehaviour>(out var customGameModeBehaviour))
        {
            customGameMode = customGameModeBehaviour.CustomGameMode;
            return true;
        }

        customGameMode = null;
        return false;
    }

    public static bool TryGetActiveCustomGameModeFlow(
        this GameManager gameManager,
        [NotNullWhen(true)] out BaseCustomGameModeFlow? customGameModeFlow)
    {
        if (!gameManager.TryGetActiveCustomGameMode(out var customGameMode) || customGameMode.Flow == null)
        {
            customGameModeFlow = null;
            return false;
        }

        customGameModeFlow = customGameMode.Flow;
        return true;
    }
}
