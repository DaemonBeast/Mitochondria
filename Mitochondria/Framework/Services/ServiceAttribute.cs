using System.Reflection;
using BepInEx;
using Mitochondria.Api.Services;

namespace Mitochondria.Framework.Services;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : Attribute
{
    public static void Register(Assembly assembly, PluginInfo pluginInfo)
    {
        foreach (var type in assembly.GetTypes())
        {
            var attribute = type.GetCustomAttribute<ServiceAttribute>();
            if (attribute == null)
            {
                continue;
            }

            if (!type.IsAssignableTo(typeof(IService)))
            {
                throw new Exception(
                    $"Type {type.Name} has {nameof(ServiceAttribute)} but doesn't extend {nameof(IService)}");
            }
            
            ServiceManager.Instance.Register(type, pluginInfo);
        }
    }
}