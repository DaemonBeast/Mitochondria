using System.Runtime.CompilerServices;
using Il2CppInterop.Runtime;
using Mitochondria.Api.UI.Flex;
using Mitochondria.Framework.UI.Flex.Factories;
using Mitochondria.Framework.Utilities;
using Mitochondria.Framework.Utilities.Extensions;
using Reactor.Utilities;
using UnityEngine;

namespace Mitochondria.Framework.UI.Flex;

public class FlexGameObjectManager
{
    public static FlexGameObjectManager Instance => Singleton<FlexGameObjectManager>.Instance;

    public List<IFlexFactory> FlexFactories { get; }

    private readonly ConditionalWeakTable<GameObject, IFlex> _flexibleObjects;

    private FlexGameObjectManager()
    {
        FlexFactories = new List<IFlexFactory>();
        
        _flexibleObjects = new ConditionalWeakTable<GameObject, IFlex>();
    }

    public void DefineFlex<TMonoBehaviour>(
        IFlexFactory<GameObject>.FlexFactoryHandler flexFactoryHandler,
        string? onScene = null,
        string? rootGameObjectName = null,
        int recursionDepth = int.MaxValue)
        where TMonoBehaviour : MonoBehaviour
    {
        FlexFactories.Add(
            new MonoBehaviourFlexFactory(
                flexFactoryHandler,
                Il2CppType.Of<TMonoBehaviour>(),
                onScene,
                rootGameObjectName,
                recursionDepth));
    }

    public void DefineFlex(
        IFlexFactory<GameObject>.FlexFactoryHandler flexFactoryHandler,
        string gameObjectName,
        string? onScene = null,
        string? rootGameObjectName = null,
        int recursionDepth = int.MaxValue)
    {
        FlexFactories.Add(
            new GameObjectFlexFactory(flexFactoryHandler, gameObjectName, onScene, rootGameObjectName, recursionDepth));
    }

    public void DefineFlex(
        IFlexFactory<GameObject>.FlexFactoryHandler flexFactoryHandler,
        string rootGameObjectName,
        string? onScene = null,
        int recursionDepth = 1)
    {
        FlexFactories.Add(
            new GameObjectFlexFactory(flexFactoryHandler, null, onScene, rootGameObjectName, recursionDepth));
    }

    public TFlex SetFlex<TFlex>(GameObject gameObject, TFlex flex)
        where TFlex : IFlex
    {
        _flexibleObjects.Add(gameObject, flex);

        return flex;
    }

    public IFlex? GetFlex(GameObject gameObject)
    {
        var flex = GetExistingFlex(gameObject);

        if (flex != null)
        {
            return flex;
        }
        
        var flexFactory = FlexFactories.LastOrDefault(
            f => f is IFlexFactory<GameObject> flexFactory && flexFactory.Matches(gameObject));

        if (flexFactory == null)
        {
            return null;
        }

        flex = ((IFlexFactory<GameObject>) flexFactory).Create(gameObject);
        _flexibleObjects.Add(gameObject, flex);

        return flex;
    }

    public IFlex? GetExistingFlex(GameObject gameObject)
    {
        return _flexibleObjects
            .FirstOrDefault(e => Il2CppEqualityComparer<GameObject>.Instance.Equals(e.Key, gameObject))
            .DefaultToNull()
            ?.Value;
    }
}