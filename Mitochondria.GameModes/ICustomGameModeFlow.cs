using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Mitochondria.GameModes;

public interface ICustomGameModeFlow
{
    public bool ShouldEndGame([NotNullWhen(true)] out GameOverReason? gameOverReason);

    public IEnumerator CoBeforeGameStart();

    public void AfterGameStart();

    public void BeforeGameEnd();

    public void AfterGameEnd();
}
