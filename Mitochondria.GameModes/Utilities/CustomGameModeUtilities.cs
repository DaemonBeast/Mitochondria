using System.Diagnostics.CodeAnalysis;
using Mitochondria.GameModes.Utilities.Extensions;

namespace Mitochondria.GameModes.Utilities;

public static class CustomGameModeUtilities
{
    public static bool TryGetActiveGameMode([NotNullWhen(true)] out BaseCustomGameMode? customGameMode)
    {
        var gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            customGameMode = null;
            return false;
        }

        return GameManager.Instance.TryGetActiveCustomGameMode(out customGameMode);
    }

    public static bool TryGetActiveGameModeFlow([NotNullWhen(true)] out BaseCustomGameModeFlow? customGameModeFlow)
    {
        var gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            customGameModeFlow = null;
            return false;
        }

        return GameManager.Instance.TryGetActiveCustomGameModeFlow(out customGameModeFlow);
    }
}
