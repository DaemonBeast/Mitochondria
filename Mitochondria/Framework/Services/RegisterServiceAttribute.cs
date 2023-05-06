using System.Reflection;
using BepInEx;
using Mitochondria.Api.Services;
using Mitochondria.Framework.Utilities;

namespace Mitochondria.Framework.Services;

[AttributeUsage(AttributeTargets.Class)]
public class RegisterServiceAttribute : Attribute
{
    public static void Register(Assembly assembly, PluginInfo pluginInfo)
    {
        TypeUtilities.RegisterAttribute<RegisterServiceAttribute, IService>(
            assembly,
            type => ServiceManager.Instance.Register(type, pluginInfo));
    }
}