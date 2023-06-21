using System.Reflection;
using BepInEx;
using Mitochondria.Api.Services;
using Mitochondria.Framework.Utilities;

namespace Mitochondria.Framework.Services;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : Attribute
{
    public static void Register(Assembly assembly, PluginInfo pluginInfo)
    {
        TypeUtils.RegisterAttribute<ServiceAttribute, IService>(
            assembly,
            type => ServiceManager.Instance.Register(type, pluginInfo));
    }
}