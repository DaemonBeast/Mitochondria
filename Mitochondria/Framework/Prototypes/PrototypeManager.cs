using System.Diagnostics.CodeAnalysis;
using Mitochondria.Framework.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mitochondria.Framework.Prototypes;

public class PrototypeManager
{
    public static PrototypeManager Instance => Singleton<PrototypeManager>.Instance;

    private readonly Dictionary<Type, MonoBehaviour> _prototypes;

    private PrototypeManager()
    {
        _prototypes = new Dictionary<Type, MonoBehaviour>();
    }
    
    public void CloneAndSet<TMonoBehaviour>(TMonoBehaviour prototypeToClone)
        where TMonoBehaviour : MonoBehaviour
        => CloneAndSet(typeof(TMonoBehaviour), prototypeToClone);

    public void CloneAndSet(Type monoBehaviourType, MonoBehaviour prototypeToClone)
    {
        var clonedPrototype = Object.Instantiate(prototypeToClone);
        clonedPrototype.gameObject.DontDestroy().SetActive(false);
        
        _prototypes[monoBehaviourType] = clonedPrototype;
    }

    public bool TryCloneAndGet<TMonoBehaviour>(
        [NotNullWhen(true)] out TMonoBehaviour? copy,
        Transform? parent = null)
        where TMonoBehaviour : MonoBehaviour
    {
        var result = TryCloneAndGet(typeof(TMonoBehaviour), out var monoBehaviour, parent);
        copy = result ? monoBehaviour!.Cast<TMonoBehaviour>() : null;

        return result;
    }

    public bool TryCloneAndGet(
        Type monoBehaviourType,
        [NotNullWhen(true)] out MonoBehaviour? copy,
        Transform? parent = null)
    {
        if (_prototypes.TryGetValue(monoBehaviourType, out var prototypeToClone))
        {
            copy = parent == null
                ? Object.Instantiate(prototypeToClone)
                : Object.Instantiate(prototypeToClone, parent);

            copy.gameObject.SetActive(true);

            return true;
        }

        copy = null;
        return false;
    }
}