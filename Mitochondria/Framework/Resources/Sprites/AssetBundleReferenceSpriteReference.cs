using Mitochondria.Framework.Resources.AssetBundles;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Framework.Resources.Sprites;

public class AssetBundleReferenceSpriteReference : SpriteReference
{
    public AssetBundleReference AssetBundleReference { get; }
    
    public string Name { get; }

    private Sprite? _sprite;

    public AssetBundleReferenceSpriteReference(AssetBundleReference assetBundleReference, string name)
    {
        AssetBundleReference = assetBundleReference;
        Name = name;
    }
    
    public override Sprite Load(bool useCached = true)
    {
        if (useCached && _sprite != null)
        {
            return _sprite;
        }

        return _sprite = AssetBundleReference.Load(useCached).LoadAsset<Sprite>(Name)!;
    }
    
    public override int GetHashCode()
    {
        return (typeof(AssetBundleReferenceSpriteReference), AssetBundleReference, Name).GetHashCode();
    }
}