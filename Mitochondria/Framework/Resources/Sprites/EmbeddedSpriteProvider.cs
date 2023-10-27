using System.Reflection;
using UnityEngine;

namespace Mitochondria.Framework.Resources.Sprites;

public class EmbeddedSpriteProvider : SpriteProvider
{
    public Assembly SourceAssembly { get; }
    
    public string Name { get; }
    
    public float Ppi { get; }

    private Sprite? _sprite;

    public EmbeddedSpriteProvider(Assembly sourceAssembly, string fullName, float ppi)
    {
        SourceAssembly = sourceAssembly;
        Name = fullName;
        Ppi = ppi;
    }

    public EmbeddedSpriteProvider(
        string name,
        float ppi = 100f,
        bool searchRecursively = true,
        Assembly? sourceAssembly = null)
    {
        var assembly = sourceAssembly ?? Assembly.GetCallingAssembly();

        SourceAssembly = assembly;
        Name = EmbeddedResourceHelper.TryGetMatchingName(assembly, name, searchRecursively);
        Ppi = ppi;
    }

    public override Sprite Load(bool useCached = true)
    {
        if (useCached && _sprite != null)
        {
            return _sprite;
        }

        using var imageStream = SourceAssembly.GetManifestResourceStream(Name)!;

        return _sprite = SpriteHelper.CreateSprite(imageStream, Ppi);
    }
    
    public override int GetHashCode()
    {
        return (typeof(AssetBundleSpriteProvider), SourceAssembly, Name, Ppi).GetHashCode();
    }
}