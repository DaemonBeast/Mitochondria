using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mitochondria.Framework.Cache;

public static class MonoBehaviourProvider
{
    public static Dictionary<Type, MonoBehaviour> Cache { get; }

    static MonoBehaviourProvider()
    {
        Cache = new Dictionary<Type, MonoBehaviour>();
    }

    /// <summary>
    /// Gets a cached instance of <typeparamref name="TMonoBehaviour" /> if available,
    /// or finds one and caches it if possible.
    /// </summary>
    /// <param name="monoBehaviour">
    /// An instance of <typeparamref name="TMonoBehaviour" />
    /// that was cached or found.
    /// </param>
    /// <typeparam name="TMonoBehaviour">The desired <see cref="MonoBehaviour" /> type.</typeparam>
    /// <returns>If an instance of <typeparamref name="TMonoBehaviour" /> was available.</returns>
    /// <remarks>
    /// The same instance is not guaranteed for requests of the same <typeparamref name="TMonoBehaviour" />.
    /// </remarks>
    public static bool TryGet<TMonoBehaviour>([NotNullWhen(true)] out TMonoBehaviour? monoBehaviour)
        where TMonoBehaviour : MonoBehaviour
    {
        if (Cache.TryGetValue(typeof(TMonoBehaviour), out var m) && m != null)
        {
            monoBehaviour = (TMonoBehaviour) m;
            return true;
        }

        monoBehaviour = Object.FindObjectOfType<TMonoBehaviour>();
        if (monoBehaviour == null)
        {
            return false;
        }

        Cache[typeof(TMonoBehaviour)] = monoBehaviour;
        
        return true;
    }

    public static void Set<TMonoBehaviour>(TMonoBehaviour monoBehaviour)
        where TMonoBehaviour : MonoBehaviour
    {
        Cache[typeof(TMonoBehaviour)] = monoBehaviour;
    }
}