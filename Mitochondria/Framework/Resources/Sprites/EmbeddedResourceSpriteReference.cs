using System.Reflection;
using UnityEngine;

namespace Mitochondria.Framework.Resources.Sprites;

public class EmbeddedResourceSpriteReference : SpriteReference
{
    public Assembly SourceAssembly { get; }
    
    public string Name { get; }
    
    public float Ppi { get; }

    private Sprite? _sprite;

    public EmbeddedResourceSpriteReference(Assembly sourceAssembly, string name, float ppi)
    {
        SourceAssembly = sourceAssembly;
        Name = name;
        Ppi = ppi;
    }

    public override Sprite Load(bool useCached = true)
    {
        if (useCached && _sprite != null)
        {
            return _sprite;
        }

        var imageStream = SourceAssembly.GetManifestResourceStream(Name)!;

        return _sprite = SpriteHelper.CreateSprite(imageStream, Ppi);
    }
    
    public override int GetHashCode()
    {
        return (typeof(AssetBundleReferenceSpriteReference), SourceAssembly, Name, Ppi).GetHashCode();
    }
}