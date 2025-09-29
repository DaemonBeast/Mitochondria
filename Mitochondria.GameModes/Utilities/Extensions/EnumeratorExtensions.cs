using System.Collections;

namespace Mitochondria.GameModes.Utilities.Extensions;

public static class EnumeratorExtensions
{
    public static IEnumerator Track(this IEnumerator enumerator, out Task task)
    {
        var taskCompletionSource = new TaskCompletionSource();
        task = taskCompletionSource.Task;

        return Wrapper();

        IEnumerator Wrapper()
        {
            yield return enumerator;
            taskCompletionSource.SetResult();
        }
    }
}
