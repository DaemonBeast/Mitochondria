using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Mitochondria.Core.Framework.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Core.Framework.Resources;

public static class SpriteHelper
{
    private static readonly Vector2 PivotCenter;

    static SpriteHelper()
    {
        PivotCenter = new Vector2(0.5f, 0.5f);
    }

    public static Sprite CreateSprite(Stream imageStream, float ppi)
    {
        return CreateSprite(imageStream.AsBytes(), ppi);
    }

    public static Sprite CreateSprite(Il2CppStructArray<byte> imageData, float ppi)
    {
        var texture = CreateEmptyTexture();
        ImageConversion.LoadImage(texture, imageData);

        return CreateSprite(texture, ppi);
    }

    public static Sprite CreateSprite(Texture2D texture, float ppi)
    {
        return Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            PivotCenter,
            ppi,
            0,
            SpriteMeshType.FullRect);
    }

    public static Texture2D CreateEmptyTexture()
    {
        return new Texture2D(0, 0, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp
        };
    }
}