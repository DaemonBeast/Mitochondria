using System.Reflection;
using Mitochondria.Api.Resources;
using UnityEngine;

namespace Mitochondria.Framework.Resources.AssetBundles;

public abstract class AssetBundleReference : ResourceReference<AssetBundle>
{
    public static AssetBundleReference Shapes { get; }

    static AssetBundleReference()
    {
        Shapes = FromEmbeddedResource("shapes");
    }
    
    public static EmbeddedResourceAssetBundleReference FromEmbeddedResource(
        string name,
        bool searchRecursively = true,
        Assembly? sourceAssembly = null)
    {
        var assembly = sourceAssembly ?? Assembly.GetCallingAssembly();

        return new EmbeddedResourceAssetBundleReference(
            assembly,
            EmbeddedResourceHelper.TryGetMatchingFullName(assembly, name, searchRecursively));
    }
    
    // TODO: AssetBundleReference from file system
}