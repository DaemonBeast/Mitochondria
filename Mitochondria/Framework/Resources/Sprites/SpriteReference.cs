using System.Reflection;
using Mitochondria.Api.Resources;
using Mitochondria.Framework.Resources.AssetBundles;
using UnityEngine;

namespace Mitochondria.Framework.Resources.Sprites;

public abstract partial class SpriteReference : ResourceReference<Sprite>
{
    public static SpriteReference Empty { get; }
    
    public static SpriteReference BlackDot { get; }

    public static SpriteReference WhiteDot { get; }

    public static SpriteReference Circle { get; }

    public static SpriteReference RoundedBox { get; }

    public static SpriteReference BorderedRoundedBox { get; }

    public static SpriteReference Badge { get; }
    
    static SpriteReference()
    {
        Empty = new EmptySpriteReference();
        
        BlackDot = FromEmbeddedResource("BlackDot.png", 1f);
        WhiteDot = FromEmbeddedResource("WhiteDot.png", 1f);
        Circle = FromEmbeddedResource("Circle.png", 508f);
        
        RoundedBox = FromBundleReference(AssetBundleReference.Shapes, "RoundedBox");
        BorderedRoundedBox = FromBundleReference(AssetBundleReference.Shapes, "BorderedRoundedBox");
        Badge = FromBundleReference(AssetBundleReference.Shapes, "Badge");
    }
    
    public static EmbeddedResourceSpriteReference FromEmbeddedResource(
        string name,
        float ppi = 100f,
        bool searchRecursively = true,
        Assembly? sourceAssembly = null)
    {
        var assembly = sourceAssembly ?? Assembly.GetCallingAssembly();
        
        return new EmbeddedResourceSpriteReference(
            assembly,
            EmbeddedResourceHelper.TryGetMatchingFullName(assembly, name, searchRecursively),
            ppi);
    }

    public static AssetBundleReferenceSpriteReference FromBundleReference(
        AssetBundleReference bundleReference,
        string name)
    {
        return new AssetBundleReferenceSpriteReference(bundleReference, name);
    }
    
    // TODO: SpriteReference from file system
}