using System.Diagnostics.CodeAnalysis;
using Mitochondria.Framework.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mitochondria.Framework.Options.SettingsOptions.Managers;

public class SettingsOptionPrototypeManager
{
    public static SettingsOptionPrototypeManager Instance => Singleton<SettingsOptionPrototypeManager>.Instance;

    private readonly Dictionary<Type, OptionBehaviour> _prototypes;

    private SettingsOptionPrototypeManager()
    {
        _prototypes = new Dictionary<Type, OptionBehaviour>();
    }

    public void CloneAndSet<TOptionBehaviour>(OptionBehaviour prototypeToClone)
        where TOptionBehaviour : OptionBehaviour
        => CloneAndSet(typeof(TOptionBehaviour), prototypeToClone);

    public void CloneAndSet(Type optionBehaviourType, OptionBehaviour prototypeToClone)
    {
        var clonedPrototype = Object.Instantiate(prototypeToClone);
        clonedPrototype.gameObject.DontDestroy().SetActive(false);
        
        _prototypes[optionBehaviourType] = clonedPrototype;
    }

    public bool TryCloneAndGet<TOptionBehaviour>(
        [NotNullWhen(true)] out TOptionBehaviour? copy,
        Transform? parent = null)
        where TOptionBehaviour : OptionBehaviour
    {
        var result = TryCloneAndGet(typeof(TOptionBehaviour), out var optionBehaviour, parent);
        copy = result ? optionBehaviour!.Cast<TOptionBehaviour>() : null;

        return result;
    }

    public bool TryCloneAndGet(
        Type optionBehaviourType,
        [NotNullWhen(true)] out OptionBehaviour? copy,
        Transform? parent = null)
    {
        if (_prototypes.TryGetValue(optionBehaviourType, out var prototypeToClone))
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