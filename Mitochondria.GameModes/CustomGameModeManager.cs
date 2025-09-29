using Mitochondria.GameModes.Utilities.Extensions;

namespace Mitochondria.GameModes;

/// <summary>
/// Manages custom game modes.
/// </summary>
public static class CustomGameModeManager
{
    /// <summary>
    /// Gets the registered game modes.
    /// </summary>
    public static IReadOnlyDictionary<AmongUs.GameOptions.GameModes, BaseCustomGameMode> GameModes => InternalGameModes;

    private static readonly Dictionary<AmongUs.GameOptions.GameModes, BaseCustomGameMode> InternalGameModes = new();

    private static byte _lastId = byte.MaxValue;

    /// <summary>
    /// Gets the next free `GameModes` ID.
    /// </summary>
    /// <returns>A `GameModes` ID.</returns>
    public static AmongUs.GameOptions.GameModes NextId()
        => (AmongUs.GameOptions.GameModes) _lastId--;

    /// <summary>
    /// Register a game mode.
    /// </summary>
    /// <typeparam name="TCustomGameMode">The game mode to register.</typeparam>
    /// <typeparam name="TCustomGameModeFlow">The game mode flow.</typeparam>
    public static void Register<TCustomGameMode, TCustomGameModeFlow>()
        where TCustomGameMode : BaseCustomGameMode<TCustomGameModeFlow>, new()
        where TCustomGameModeFlow : BaseCustomGameModeFlow, new()
    {
        if (GameModes.Values.FirstOrDefault(customGameMode => customGameMode is TCustomGameMode)
            is { } matchingCustomGameMode)
        {
            Warning(
                $"Tried to register the game mode \"{TranslationController.Instance.GetString(matchingCustomGameMode.Name)}\" but it was already registered.");

            return;
        }

        var newCustomGameMode = new TCustomGameMode();
        ConfigureNewGameMode(newCustomGameMode);
    }

    internal static void Register(Type customGameModeType)
    {
        if (GameModes.Values.FirstOrDefault(customGameMode => customGameMode.GetType() == customGameModeType) is
            { } existingCustomGameMode)
        {
            Warning(
                $"Tried to register the game mode \"{TranslationController.Instance.GetString(existingCustomGameMode.Name)}\" but it was already registered.");

            return;
        }

        var newCustomGameMode = (BaseCustomGameMode) Activator.CreateInstance(customGameModeType)!;
        ConfigureNewGameMode(newCustomGameMode);
    }

    private static void ConfigureNewGameMode(BaseCustomGameMode newCustomGameMode)
    {
        var gameMode = NextId();
        InternalGameModes.Add(gameMode, newCustomGameMode);

        var builder = new CustomGameModeConfigurationBuilder(newCustomGameMode.Configuration);

        var baseTypes = newCustomGameMode.GetType()
            .GetBaseTypes()
            .TakeWhile(potentialType => potentialType is { IsGenericType: false, IsAbstract: false })
            .Reverse();

        foreach (var baseType in baseTypes)
        {
            var tempCustomGameMode = (BaseCustomGameMode) Activator.CreateInstance(baseType)!;
            tempCustomGameMode.OnConfigure(builder);
        }

        newCustomGameMode.OnConfigure(builder);
        newCustomGameMode.InternalConfiguration = builder.Build();

        Debug($"Registered game mode \"{TranslationController.Instance.GetString(newCustomGameMode.Name)}\".");
    }
}
