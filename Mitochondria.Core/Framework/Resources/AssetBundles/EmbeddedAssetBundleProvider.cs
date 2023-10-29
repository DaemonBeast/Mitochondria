using System.Collections;
using System.Reflection;
using Mitochondria.Core.Framework.Utilities.Extensions;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Core.Framework.Resources.AssetBundles;

public class EmbeddedAssetBundleProvider : AssetBundleProvider
{
    public Assembly SourceAssembly { get; }
    
    public string Name { get; }

    private AssetBundle? _bundle;

    public EmbeddedAssetBundleProvider(Assembly sourceAssembly, string fullName)
    {
        SourceAssembly = sourceAssembly;
        Name = fullName;
    }

    public EmbeddedAssetBundleProvider(string name, bool searchRecursively = true, Assembly? sourceAssembly = null)
    {
        var assembly = sourceAssembly ?? Assembly.GetCallingAssembly();

        SourceAssembly = assembly;
        Name = EmbeddedResourceHelper.TryGetMatchingName(assembly, name, searchRecursively);
    }

    public override AssetBundle Load(bool useCached = true)
    {
        if (useCached && _bundle != null)
        {
            return _bundle;
        }
        
        using var assetBundleStream = SourceAssembly.GetManifestResourceStream(Name)!;
        _bundle = AssetBundle.LoadFromStream(assetBundleStream.AsIl2Cpp());

        return _bundle;
    }

    public override IEnumerator CoLoad(bool skipIfCached = true, Action<AssetBundle>? onCompleted = null)
    {
        if (skipIfCached && _bundle != null)
        {
            yield break;
        }

        var assetBundleBytes = SourceAssembly.GetManifestResourceStream(Name)!.AsBytes();
        var request = AssetBundle.LoadFromMemoryAsync(assetBundleBytes);

        request.add_completed((Action<AsyncOperation>) (_ =>
        {
            _bundle = request.assetBundle;
            onCompleted?.Invoke(_bundle);
        }));
    }

    public override int GetHashCode()
    {
        return (typeof(EmbeddedAssetBundleProvider), SourceAssembly, Name).GetHashCode();
    }
}