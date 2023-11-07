using Il2CppInterop.Runtime;
using UnityEngine;

namespace Mitochondria.Core.Framework.Utilities.Extensions;

public static class GameObjectExtensions
{
    public static IEnumerable<GameObject> GetImmediateChildren(this GameObject parent)
    {
        foreach (var transform in parent.transform)
        {
            yield return transform.Cast<Transform>().gameObject;
        }
    }

    public static IEnumerable<GameObject> GetChildren(this GameObject parent, int recursionDepth = int.MaxValue)
    {
        foreach (var t in parent.transform)
        {
            var gameObject = t.Cast<Transform>().gameObject;
            yield return gameObject;

            if (recursionDepth > 1)
            {
                foreach (var g in gameObject.GetChildren(recursionDepth - 1))
                {
                    yield return g;
                }
            }
        }
    }

    public static bool HasParentWithName(this GameObject gameObject, string name, int recursionDepth = int.MaxValue)
    {
        return GetParents(gameObject, recursionDepth).Any(g => g.name == name);
    }

    public static IEnumerable<GameObject> GetParents(this GameObject gameObject, int recursionDepth = int.MaxValue)
    {
        var transform = gameObject.transform.parent;
        var i = 0;

        while (transform != null && i++ < recursionDepth)
        {
            yield return transform.gameObject;
            transform = transform.parent;
        }
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject)
        where T : MonoBehaviour
    {
        return gameObject.TryGetComponent(Il2CppType.Of<T>(), out var component)
            ? component.Cast<T>()
            : gameObject.AddComponent<T>();
    }
}