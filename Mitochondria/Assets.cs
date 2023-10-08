using System.Reflection;
using Mitochondria.Framework.Resources.AssetBundles;
using Mitochondria.Framework.Resources.Sprites;

namespace Mitochondria;

public static class Assets
{
    private static Assembly SourceAssembly { get; }

    static Assets()
    {
        SourceAssembly =  typeof(Assets).Assembly;
    }

    public static class AssetBundles
    {
        public static EmbeddedAssetBundleProvider Shapes { get; }

        static AssetBundles()
        {
            Shapes = new EmbeddedAssetBundleProvider("shapes", true, SourceAssembly);
        }
    }

    public static class Sprites
    {
        public static EmptySpriteProvider Empty { get; }
    
        public static EmbeddedSpriteProvider BlackDot { get; }

        public static EmbeddedSpriteProvider WhiteDot { get; }

        public static EmbeddedSpriteProvider LargeCircle { get; }

        public static AssetBundleSpriteProvider RoundedBox { get; }

        public static AssetBundleSpriteProvider BorderedRoundedBox { get; }

        public static AssetBundleSpriteProvider Badge { get; }

        static Sprites()
        {
            Empty = new EmptySpriteProvider();

            BlackDot = new EmbeddedSpriteProvider("BlackDot.png", 1f, true, SourceAssembly);
            WhiteDot = new EmbeddedSpriteProvider("WhiteDot.png", 1f, true, SourceAssembly);
            LargeCircle = new EmbeddedSpriteProvider("Circle.png", 508f, true, SourceAssembly);

            RoundedBox = new AssetBundleSpriteProvider(AssetBundles.Shapes, "RoundedBox");
            BorderedRoundedBox = new AssetBundleSpriteProvider(AssetBundles.Shapes, "BorderedRoundedBox");
            Badge = new AssetBundleSpriteProvider(AssetBundles.Shapes, "Badge");
        }

        public static class Buttons
        {
            public static EmbeddedSpriteProvider Circle { get; }

            static Buttons()
            {
                Circle = new EmbeddedSpriteProvider("Circle.png", 1417f, true, SourceAssembly);
            }
        }
    }
}