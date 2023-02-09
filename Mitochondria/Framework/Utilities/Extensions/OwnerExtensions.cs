using BepInEx;
using BepInEx.Unity.IL2CPP;
using Mitochondria.Api.Owner;
using Mitochondria.Framework.Owner;
using Mitochondria.Framework.Plugin;

namespace Mitochondria.Framework.Utilities.Extensions;

public static class OwnerExtensions
{
    public static PluginInfo? GetOwner(this IOwned owned)
        => OwnerManager.Instance.GetOwner(owned);

    public static void SetOwner<TPlugin>(this IOwned owned)
        where TPlugin : BasePlugin
    {
        owned.SetOwner(PluginManager<TPlugin>.PluginInfo);
    }

    public static void SetOwner(this IOwned owned, PluginInfo owner)
        => OwnerManager.Instance.Set(owned, owner);
}