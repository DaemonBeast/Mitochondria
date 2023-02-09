using Mitochondria.Api.UI.Flex;
using Mitochondria.Framework.Utilities.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mitochondria.Framework.UI.Flex.Factories;

public class GameObjectFlexFactory : IFlexFactory<GameObject>
{
    public string? GameObjectName { get; }
    
    public string? OnScene { get; }
    
    public string? RootGameObjectName { get; }
    
    public int RecursionDepth { get; }
    
    private readonly IFlexFactory<GameObject>.FlexFactoryHandler _flexFactoryHandler;

    public GameObjectFlexFactory(
        IFlexFactory<GameObject>.FlexFactoryHandler flexFactoryHandler,
        string? gameObjectName,
        string? onScene = null,
        string? rootGameObjectName = null,
        int recursionDepth = int.MaxValue)
    {
        GameObjectName = gameObjectName;
        OnScene = onScene;
        RootGameObjectName = rootGameObjectName;
        RecursionDepth = recursionDepth;

        _flexFactoryHandler = flexFactoryHandler;
    }
    
    public virtual bool Matches(GameObject obj)
    {
        return (GameObjectName == null || GameObjectName == obj.name) &&
               (OnScene == null || SceneManager.GetActiveScene().name == OnScene) &&
               (RootGameObjectName == null || obj.HasAncestorWithName(RootGameObjectName, RecursionDepth));
    }

    public IFlex Create(GameObject gameObject)
    {
        return _flexFactoryHandler.Invoke(gameObject);
    }
}