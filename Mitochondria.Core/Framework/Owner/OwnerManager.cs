using System.Runtime.CompilerServices;
using BepInEx;
using Mitochondria.Core.Api.Owner;
using Mitochondria.Core.Framework.Utilities;

namespace Mitochondria.Core.Framework.Owner;

public class OwnerManager
{
    public static OwnerManager Instance => Singleton<OwnerManager>.Instance;

    private readonly ConditionalWeakTable<IOwned, PluginInfo> _owners;

    private readonly ConditionalWeakTable<PluginInfo, List<IOwned>> _owned;

    private OwnerManager()
    {
        _owners = new ConditionalWeakTable<IOwned, PluginInfo>();
        _owned = new ConditionalWeakTable<PluginInfo, List<IOwned>>();
    }

    public void Set(IOwned owned, PluginInfo owner)
    {
        if (_owners.TryGetValue(owned, out var oldOwner))
        {
            if (oldOwner == owner)
            {
                return;
            }
            
            _owners.AddOrUpdate(owned, owner);
            _owned.GetOrCreateValue(oldOwner).Remove(owned);
        }
        else
        {
            _owners.Add(owned, owner);
        }
        
        _owned.GetOrCreateValue(owner).Add(owned);
    }

    public PluginInfo? GetOwner(IOwned owned)
    {
        return _owners.TryGetValue(owned, out var owner) ? owner : null;
    }

    public IOwned[] GetOwned(PluginInfo owner)
    {
        return _owned.TryGetValue(owner, out var owned) ? owned.ToArray() : Array.Empty<IOwned>();
    }
}