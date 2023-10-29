using BepInEx;
using Mitochondria.Core.Api.Services;
using Mitochondria.Core.Framework.Utilities.Extensions;

namespace Mitochondria.Core.Framework.Services.Extensions;

public static class ServiceExtensions
{
    public static IService[] GetServices(this PluginInfo owner)
    {
        return ServiceManager.Instance.Services.Values.Where(s => s.GetOwner() == owner).ToArray();
    }
}