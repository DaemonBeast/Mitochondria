using Mitochondria.Core.Framework.Resources.AssetBundles;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Core.Framework.Resources.Sprites;

public class AssetBundleSpriteProvider : SpriteProvider
{
    public AssetBundleProvider AssetBundleProvider { get; }
    
    public string Name { get; }

    private Sprite? _sprite;

    public AssetBundleSpriteProvider(AssetBundleProvider assetBundleProvider, string name)
    {
        AssetBundleProvider = assetBundleProvider;
        Name = name;
    }
    
    public override Sprite Load(bool useCached = true)
    {
        if (useCached && _sprite != null)
        {
            return _sprite;
        }

        return _sprite = AssetBundleProvider.Load(useCached).LoadAsset<Sprite>(Name)!;
    }
    
    public override int GetHashCode()
    {
        return (typeof(AssetBundleSpriteProvider), AssetBundleReference: AssetBundleProvider, Name).GetHashCode();
    }
}