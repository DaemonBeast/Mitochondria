using System.Collections;

namespace Mitochondria.GameModes;

public abstract class BaseCustomGameModeFlow : IDisposable
{
    public virtual CanStartGameResult CanStartGame()
        => CanStartGameResult.True;

    public abstract ShouldEndGameResult ShouldEndGame();

    public virtual IEnumerator CoSetupGame()
    {
        yield break;
    }

    public virtual IEnumerator CoAfterGameStart()
    {
        yield break;
    }

    public virtual void BeforeGameEnd()
    {
    }

    public virtual void AfterGameEnd()
    {
    }

    public abstract void Dispose();

    public class CanStartGameResult
    {
        public static CanStartGameResult True { get; } = new(true);

        public bool CanStartGame { get; }
        public string? Explanation { get; }

        public static CanStartGameResult False(string explanation)
            => new(false, explanation);

        private CanStartGameResult(bool canStartGame, string? explanation = null)
        {
            CanStartGame = canStartGame;
            Explanation = explanation;
        }
    }

    public class ShouldEndGameResult
    {
        public static ShouldEndGameResult False { get; } = new(false);

        public bool ShouldEndGame { get; }
        public GameOverReason? Reason { get; }

        public static ShouldEndGameResult True(GameOverReason reason)
            => new(true, reason);

        private ShouldEndGameResult(bool shouldEndGame, GameOverReason? reason = null)
        {
            ShouldEndGame = shouldEndGame;
            Reason = reason;
        }
    }
}
