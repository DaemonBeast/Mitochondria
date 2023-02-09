using Mitochondria.Api.UI.Flex;
using Mitochondria.Framework.UI.Flex;
using UnityEngine;

namespace Mitochondria.Framework.UI.Extensions;

public static class FlexExtensions
{
    public static TFlex SetFlex<TFlex>(this GameObject gameObject, TFlex flex)
        where TFlex : IFlex
    {
        return FlexGameObjectManager.Instance.SetFlex(gameObject, flex);
    }

    public static IFlex? AsFlex(this GameObject gameObject)
        => FlexGameObjectManager.Instance.GetFlex(gameObject);
    
    public static IFlex? AsExistingFlex(this GameObject gameObject)
        => FlexGameObjectManager.Instance.GetExistingFlex(gameObject);
}