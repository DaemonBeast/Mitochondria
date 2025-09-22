namespace Mitochondria.GameModes;

public interface ICustomGameMode
{
    public CustomGameModeConfiguration Configuration { get; internal set; }

    public StringNames Name { get; }

    public ICustomGameModeFlow Flow { get; }

    public void OnConfigure(CustomGameModeConfigurationBuilder configurationBuilder);

    public void Initialize();

    public void Deinitialize();

    /*public IEnumerator CoIntroCutscene();

    public IEnumerator CoEndCutscene();*/
}

/*public class MyCustomGameMode2 : ICustomGameMode
{
    public StringNames Name { get; } = CustomStringName.CreateAndRegister("Zombies Story Mode");

    public void Initialize()
    {
        
    }

    public void Deinitialize()
    {
        
    }

    public IEnumerator CoIntroCutscene()
    {
        // Set up cutscene

        /*yield return new WaitForCueToStartCutscene();

        // Play cutscene

        yield return new WaitForOthersToFinishCutscene();

        // Do some clean up, prepare stuff, etc.

        yield return new AllowGameToStart();*/

        // Clean up the rest of the stuff

        /*yield break;
    }

    public IEnumerator CoEndCutscene()
    {
        throw new System.NotImplementedException();
    }

    public bool ShouldEndGame([NotNullWhen(true)] out GameOverReason? gameOverReason)
    {
        gameOverReason = null;
        return false;
    }
}*/

// TODO: Gamemode API:
//     Initialize
//     Deinitialize
//     CoCutscene? Or SetupCutscene then CleanUpCutscene? Or PrepareGame?

/*public class WaitForCueToStartCutscene;
public class WaitForOthersToFinishCutscene;
public class AllowGameToStart;*/
