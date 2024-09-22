using UnityEngine;

namespace Mitochondria.Resources.Utilities;

public static class SpriteUtils
{
    private static readonly Vector2 PivotCenter = new(0.5f, 0.5f);

    public static Sprite CreateSprite(Texture2D texture, float pixelsPerUnit)
    {
        return Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            PivotCenter,
            pixelsPerUnit,
            0,
            SpriteMeshType.FullRect);
    }
}
