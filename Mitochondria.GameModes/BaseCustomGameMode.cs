namespace Mitochondria.GameModes;

public abstract class BaseCustomGameMode<TCustomGameModeFlow> : BaseCustomGameMode
    where TCustomGameModeFlow : BaseCustomGameModeFlow, new()
{
    internal sealed override BaseCustomGameModeFlow CreateFlow()
        => new TCustomGameModeFlow();
}

public abstract class BaseCustomGameMode
{
    public CustomGameModeConfiguration Configuration => InternalConfiguration;

    public abstract StringNames Name { get; }

    internal BaseCustomGameModeFlow? Flow { get; set; }

    internal CustomGameModeConfiguration InternalConfiguration = CustomGameModeConfiguration.Default;

    public virtual void OnConfigure(CustomGameModeConfigurationBuilder configurationBuilder)
    {
    }

    internal abstract BaseCustomGameModeFlow CreateFlow();
}
