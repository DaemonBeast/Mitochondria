using UnityEngine;

namespace Mitochondria.Framework.Helpers;

public static class HudManagerHelper
{
    public static HudManagerTransforms Transforms { get; }

    static HudManagerHelper()
    {
        Transforms = new HudManagerTransforms();
    }
}

public class HudManagerTransforms
{
    public Transform? BottomRightButtons { get; internal set; }
}