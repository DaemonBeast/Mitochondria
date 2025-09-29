using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Mitochondria.GameModes.Utilities.Extensions;
using Reactor.Utilities;
using UnityEngine;

namespace Mitochondria.GameModes.Utilities;

public static class EnumeratorUtilities
{
    public static IEnumerator WhenAll(params IEnumerator[] enumerators)
        => WhenAll(coroutine => Coroutines.Start(coroutine), enumerators);

    public static IEnumerator WhenAll(
        Func<Il2CppSystem.Collections.IEnumerator, Coroutine> dispatcher,
        params IEnumerator[] enumerators)
        => WhenAll(enumerator => dispatcher.Invoke(enumerator.WrapToIl2Cpp()), enumerators);

    public static IEnumerator WhenAll(Action<IEnumerator> dispatcher, params IEnumerator[] enumerators)
    {
        var tasks = new Task[enumerators.Length];

        for (var i = 0; i < enumerators.Length; i++)
        {
            dispatcher.Invoke(enumerators[i].Track(out var task));
            tasks[i] = task;
        }

        var allTasks = Task.WhenAll(tasks);

        while (!allTasks.IsCompleted) yield return null;
    }

    public static IEnumerator ConcatAll(params IEnumerator[] enumerators)
        => enumerators.GetEnumerator();

    public static IEnumerator Empty()
    {
        yield break;
    }
}
