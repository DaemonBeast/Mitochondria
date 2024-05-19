using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Mitochondria.Core.Framework.Utilities.Extensions;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Core.Framework.Resources.AssetBundles;

public class EmbeddedAssetBundleProvider : AssetBundleProvider
{
    public Assembly SourceAssembly { get; }
    
    public string Name { get; }

    private static readonly Dictionary<string, AssetBundle> _loadedBundles = new Dictionary<string, AssetBundle>();

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
        
        if (useCached && _loadedBundles.TryGetValue(Name, out var cachedBundle))
        {
            _bundle = cachedBundle;
            return _bundle;
        }

        using var assetBundleStream = SourceAssembly.GetManifestResourceStream(Name)!;
        _bundle = AssetBundle.LoadFromStream(assetBundleStream.AsIl2Cpp());

        _loadedBundles[Name] = _bundle;

        return _bundle;
    }

    public override IEnumerator CoLoad(bool skipIfCached = true, Action<AssetBundle>? onCompleted = null)
    {
        if (skipIfCached && _bundle != null)
        {
            yield break;
        }

        if (skipIfCached && _loadedBundles.TryGetValue(Name, out var cachedBundle))
        {
            _bundle = cachedBundle;
            onCompleted?.Invoke(_bundle);
            yield break;
        }

        var assetBundleBytes = SourceAssembly.GetManifestResourceStream(Name)!.AsBytes();
        var request = AssetBundle.LoadFromMemoryAsync(assetBundleBytes);

        request.add_completed((Action<AsyncOperation>) (_ =>
        {
            _bundle = request.assetBundle;
            _loadedBundles[Name] = _bundle;
            onCompleted?.Invoke(_bundle);
        }));
    }

    public override int GetHashCode()
    {
        return (typeof(EmbeddedAssetBundleProvider), SourceAssembly, Name).GetHashCode();
    }
}
