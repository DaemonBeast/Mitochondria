namespace Mitochondria.GameModes.Utilities.Extensions;

public static class GameModeExtensions
{
    public static StringNames GetStringName(this AmongUs.GameOptions.GameModes gameMode)
        => CustomGameModeManager.GameModes.TryGetValue(gameMode, out var customGameMode)
            ? customGameMode.Name
            : GameModesHelpers.ModeToName.TryGetValue(gameMode, out var stringName)
                ? stringName
                : StringNames.GameTypeError;
}
