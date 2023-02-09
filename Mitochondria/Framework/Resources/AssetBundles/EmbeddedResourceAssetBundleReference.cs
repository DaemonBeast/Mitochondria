using System.Reflection;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Framework.Resources.AssetBundles;

public class EmbeddedResourceAssetBundleReference : AssetBundleReference
{
    public Assembly SourceAssembly { get; }
    
    public string Name { get; }

    private AssetBundle? _bundle;

    public EmbeddedResourceAssetBundleReference(Assembly sourceAssembly, string name)
    {
        SourceAssembly = sourceAssembly;
        Name = name;
    }

    public override AssetBundle Load(bool useCached = true)
    {
        if (useCached && _bundle != null)
        {
            return _bundle;
        }
        
        var assetBundleStream = SourceAssembly.GetManifestResourceStream(Name)!;

        _bundle = AssetBundle.LoadFromStream(assetBundleStream.AsIl2Cpp());

        return _bundle;
    }

    public override int GetHashCode()
    {
        return (typeof(EmbeddedResourceAssetBundleReference), SourceAssembly, Name).GetHashCode();
    }
}