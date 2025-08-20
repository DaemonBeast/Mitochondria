using System.Collections;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;

namespace Mitochondria.Utilities.Extensions;

public static class UnityObjectExtensions
{
    public static async Task DestroyAsync(this UnityEngine.Object obj)
    {
        var taskCompletionSource = new TaskCompletionSource();

        Coroutines.Start(CoDestroy(obj, () => taskCompletionSource.TrySetResult()));

        await taskCompletionSource.Task;
    }

    private static IEnumerator CoDestroy(UnityEngine.Object obj, Action onEnd)
    {
        yield return null;

        if (obj != null)
        {
            obj.Destroy();
        }

        onEnd.Invoke();
    }
}
