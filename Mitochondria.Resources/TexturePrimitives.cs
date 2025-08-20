using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Mitochondria.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Resources;

public static class TexturePrimitives
{
    public static Texture2D GenerateCircle(float radius, Color32? color = null)
    {
        var radiusSquared = radius * radius;
        var length = (int) Math.Ceiling(radius * 2);

        var texture = CreateEmptyTexture<int>(length, length, out var data, out var span);
        span.Fill(0);

        var actualColor = color ?? Color.white;
        var colorInt = Unsafe.As<Color32, int>(ref actualColor);

        var halfHeight = (int) Math.Ceiling(length / 2f);
        for (var y = 0; y < halfHeight; y++)
        {
            var translatedY = y - radius;
            var x = Math.Sqrt(radiusSquared - translatedY * translatedY);

            var minX = (int) Math.Round(radius - x);
            var maxX = (int) Math.Round(radius + x);
            var upperRow = y * length;
            var lowerRow = (length - 1 - y) * length;
            span[(upperRow + minX)..(upperRow + maxX)].Fill(colorInt);
            span[(lowerRow + minX)..(lowerRow + maxX)].Fill(colorInt);
        }

        texture.LoadRawTextureData(data);
        texture.Apply(false);
        return texture;
    }

    public static Texture2D GenerateCircle(float radius, Func<float, float, Color32> getColor)
    {
        var radiusSquared = radius * radius;
        var length = (int) Math.Ceiling(radius * 2);

        var texture = CreateEmptyTexture<Color32>(length, length, out var data, out var span);
        span.Fill(Color.clear);

        var halfHeight = (int) Math.Ceiling(length / 2f);
        for (var y = 0; y < halfHeight; y++)
        {
            var translatedY = y - radius;
            var x = Math.Sqrt(radiusSquared - translatedY * translatedY);

            var minX = (int) Math.Round(radius - x);
            var maxX = (int) Math.Round(radius + x);
            var lowerY = length - 1 - y;
            var upperRow = y * length;
            var lowerRow = lowerY * length;

            for (var i = minX; i < maxX; i++)
            {
                span[upperRow + i] = getColor(i, y);
                span[lowerRow + i] = getColor(i, lowerY);
            }
        }

        texture.LoadRawTextureData(data);
        texture.Apply(false);
        return texture;
    }

    public static Texture2D GenerateRectangle(int width, int height, Color32? color = null)
    {
        var texture = CreateEmptyTexture<int>(width, height, out var data, out var span);

        var actualColor = color ?? Color.white;
        span.Fill(Unsafe.As<Color32, int>(ref actualColor));

        texture.LoadRawTextureData(data);
        texture.Apply(false);
        return texture;
    }

    public static Texture2D GenerateRectangle(int width, int height, Func<float, float, Color32> getColor)
    {
        var texture = CreateEmptyTexture<Color32>(width, height, out var data, out var span);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                span[y * width + x] = getColor(x, y);
            }
        }

        texture.LoadRawTextureData(data);
        texture.Apply(false);
        return texture;
    }

    private static Texture2D CreateEmptyTexture<T>(
        int width,
        int height,
        out Il2CppStructArray<byte> data,
        out Span<T> span)
        where T : struct
    {
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        data = new Il2CppStructArray<byte>(width * height * 4);
        span = MemoryMarshal.Cast<byte, T>(data.AsSpan());

        return texture;
    }
}
