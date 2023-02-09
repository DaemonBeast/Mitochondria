using Reactor.Utilities;
using Object = UnityEngine.Object;

namespace Mitochondria.Framework.Utilities.Extensions;

public static class UnityObjectExtensions
{
    public static TResult? ThenIfNotNull<T, TResult>(this T? obj, Func<T, TResult?> callback)
        where T : Object
        where TResult : class
    {
        return obj == null ? null : callback.Invoke(obj);
    }
    
    public static TResult? ThenIfNotNull<T, TResult>(this T? obj, Func<T, TResult?> callback)
        where T : Object
        where TResult : struct
    {
        return obj == null ? null : callback.Invoke(obj);
    }
    
    public static void ThenIfNotNull<T>(this T? obj, Action<T> callback)
        where T : Object
    {
        if (obj != null)
        {
            callback.Invoke(obj);
        }
    }
    
    public static bool IsEqualTo<T>(this T? x, T? y)
        where T : Il2CppSystem.Object
    {
        return Il2CppEqualityComparer<T>.Instance.Equals(x, y);
    }
}