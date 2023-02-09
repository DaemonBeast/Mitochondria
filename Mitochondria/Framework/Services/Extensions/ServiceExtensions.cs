using BepInEx;
using Mitochondria.Api.Services;
using Mitochondria.Framework.Utilities.Extensions;

namespace Mitochondria.Framework.Services.Extensions;

public static class ServiceExtensions
{
    public static IService[] GetServices(this PluginInfo owner)
    {
        return ServiceManager.Instance.Services.Values.Where(s => s.GetOwner() == owner).ToArray();
    }
}