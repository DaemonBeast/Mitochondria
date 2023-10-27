using Mitochondria.Framework.Resources.Cache;
using UnityEngine;

namespace Mitochondria.Api.UI.Hud.Buttons;

public class ImpostorTextStyle : TextStyle
{
    public override Material? Material => MaterialCache.Instance.Get(MaterialCacheIds.RedAndWhiteText);
}

public class TextStyle
{
    public static TextStyle Normal { get; }

    public static TextStyle RedAndWhite { get; }

    static TextStyle()
    {
        Normal = new TextStyle();
        RedAndWhite = new ImpostorTextStyle();
    }

    public virtual Material? Material => null;
}