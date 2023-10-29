using System.Reflection;
using BepInEx;
using Mitochondria.Core.Api.Services;
using Mitochondria.Core.Framework.Utilities;

namespace Mitochondria.Core.Framework.Services;

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