using System.Collections;

namespace Mitochondria.GameModes;

public abstract class BaseCustomGameModeIntroCutscene : IDisposable
{
    public abstract IEnumerator CoIntroCutscene();

    public abstract void Dispose();
}
