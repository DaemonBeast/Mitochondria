using UnityEngine;

namespace Mitochondria.Framework.Utilities.Extensions;

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

    public static bool HasAncestorWithName(this GameObject gameObject, string name, int recursionDepth = int.MaxValue)
    {
        return GetAncestors(gameObject, recursionDepth).Any(t => t.name == name);
    }

    public static IEnumerable<Transform> GetAncestors(this GameObject gameObject, int recursionDepth = int.MaxValue)
    {
        var transform = gameObject.transform.parent;
        var i = 0;

        while (transform != null && i++ < recursionDepth)
        {
            yield return transform;

            transform = transform.parent;
        }
    }
}