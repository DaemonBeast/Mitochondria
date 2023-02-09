using UnityEngine;

namespace Mitochondria.Framework.Utilities.Extensions;

public static class GameObjectExtensions
{
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