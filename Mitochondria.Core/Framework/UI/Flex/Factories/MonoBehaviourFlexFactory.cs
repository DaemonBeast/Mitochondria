using Mitochondria.Core.Api.UI.Flex;
using UnityEngine;

namespace Mitochondria.Core.Framework.UI.Flex.Factories;

public class MonoBehaviourFlexFactory : GameObjectFlexFactory
{
    public Il2CppSystem.Type MonoBehaviourType { get; }

    public MonoBehaviourFlexFactory(
        IFlexFactory<GameObject>.FlexFactoryHandler flexFactoryHandler,
        Il2CppSystem.Type monoBehaviourType,
        string? onScene = null,
        string? rootGameObjectName = null,
        int recursionDepth = int.MaxValue) : base(flexFactoryHandler, null, onScene, rootGameObjectName, recursionDepth)
    {
        MonoBehaviourType = monoBehaviourType;
    }

    public override bool Matches(GameObject obj)
    {
        return base.Matches(obj) && obj.GetComponent(MonoBehaviourType) != null;
    }
}