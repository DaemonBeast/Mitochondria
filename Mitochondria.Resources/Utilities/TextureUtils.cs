using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Resources.Utilities;

public static class TextureUtils
{
    public static Texture2D CreateTexture(Stream imageData)
    {
        var length = imageData.Length;
        var byteData = new Il2CppStructArray<byte>(length);

        if (imageData.Read(byteData.ToSpan()) < length)
        {
            throw new Exception("Failed to read all image data");
        }

        return CreateTexture(byteData);
    }

    public static Texture2D CreateTexture(Il2CppStructArray<byte> imageData)
    {
        var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp
        };

        if (!texture.LoadImage(imageData))
        {
            throw new Exception("Failed to load image onto texture");
        }

        return texture;
    }
}
