using Mitochondria.Core.Api.UI.Flex;
using UnityEngine;

namespace Mitochondria.Core.Framework.UI.Flex.SettingsOptions;

public class SettingsOptionFlex : Api.UI.Flex.Flex
{
    // TODO: allow padding on sides and cross alignment
    
    public override BooleanBox CanBePadded => new(true, false, true, false);

    public override bool Active => GameObject != null && GameObject.activeInHierarchy;

    public override bool CanCrossAlign => false;

    public GameObject GameObject { get; }

    public SettingsOptionFlex(GameObject gameObject, int order = 0) : base(order: order)
    {
        GameObject = gameObject;
    }
}