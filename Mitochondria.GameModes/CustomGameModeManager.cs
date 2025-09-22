namespace Mitochondria.GameModes;

/// <summary>
/// Manages custom game modes.
/// </summary>
public static class CustomGameModeManager
{
    /// <summary>
    /// Gets the registered game modes.
    /// </summary>
    public static IReadOnlyDictionary<AmongUs.GameOptions.GameModes, ICustomGameMode> GameModes => InternalGameModes;

    private static readonly Dictionary<AmongUs.GameOptions.GameModes, ICustomGameMode> InternalGameModes = new();

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
    public static void Register<TCustomGameMode>()
        where TCustomGameMode : ICustomGameMode, new()
    {
        if (GameModes.Values.FirstOrDefault(customGameMode => customGameMode is TCustomGameMode)
            is { } matchingCustomGameMode)
        {
            Warning(
                $"Tried to register the game mode \"{TranslationController.Instance.GetString(matchingCustomGameMode.Name)}\" but it was already registered.");

            return;
        }

        var gameMode = NextId();
        var newCustomGameMode = new TCustomGameMode();
        InternalGameModes.Add(gameMode, newCustomGameMode);

        var builder = new CustomGameModeConfigurationBuilder(newCustomGameMode.Configuration);
        newCustomGameMode.OnConfigure(builder);
        newCustomGameMode.Configuration = builder.Build();

        Debug($"Registered game mode \"{TranslationController.Instance.GetString(newCustomGameMode.Name)}\".");
    }

    /// <summary>
    /// Register a game mode.
    /// </summary>
    /// <param name="customGameModeType">The game mode to register. Must inherit from <see cref="ICustomGameMode"/>.</param>
    internal static void Register(Type customGameModeType)
    {
        if (!typeof(ICustomGameMode).IsAssignableFrom(customGameModeType))
        {
            Warning($"{customGameModeType.Name} does not inherit {nameof(ICustomGameMode)}.");
            return;
        }

        if (GameModes.Values.Any(customGameMode => customGameMode.GetType() == customGameModeType))
        {
            Warning($"Tried to register the game mode \"{customGameModeType.Name}\" but it was already registered.");
            return;
        }

        ICustomGameMode newCustomGameMode;
        try
        {
            var nullableCustomGameMode = Activator.CreateInstance(customGameModeType);
            if (nullableCustomGameMode is not ICustomGameMode customGameMode)
            {
                Error($"Failed to instantiate the game mode \"{customGameModeType.Name}\".");
                return;
            }

            newCustomGameMode = customGameMode;
        }
        catch (Exception e)
        {
            Error($"Failed to instantiate the game mode \"{customGameModeType.Name}\": {e}");
            return;
        }

        var gameMode = NextId();
        InternalGameModes.Add(gameMode, newCustomGameMode);

        var builder = new CustomGameModeConfigurationBuilder(newCustomGameMode.Configuration);
        newCustomGameMode.OnConfigure(builder);
        newCustomGameMode.Configuration = builder.Build();

        Debug($"Registered game mode \"{TranslationController.Instance.GetString(newCustomGameMode.Name)}\".");
    }
}
