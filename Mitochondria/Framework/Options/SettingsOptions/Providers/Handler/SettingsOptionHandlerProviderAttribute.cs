using System.Reflection;
using Mitochondria.Api.Options.SettingsOptions;
using Reactor.Utilities;

namespace Mitochondria.Framework.Options.SettingsOptions.Providers.Handler;

[AttributeUsage(AttributeTargets.Class)]
public class SettingsOptionHandlerProviderAttribute : Attribute
{
    public static void Register(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            var attribute = type.GetCustomAttribute<SettingsOptionHandlerProviderAttribute>();
            if (attribute == null)
            {
                continue;
            }

            if (!type.IsAssignableTo(typeof(ISettingsOptionHandlerProvider)))
            {
                throw new Exception(
                    $"Type {type.Name} has {nameof(SettingsOptionHandlerProviderAttribute)} but doesn't extend {nameof(ISettingsOptionHandlerProvider)}");
            }

            if (Activator.CreateInstance(type, true) is ISettingsOptionHandlerProvider handlerProvider)
            {
                SettingsOptionManager.Instance.RegisterHandlerProvider(handlerProvider);
            }
            else
            {
                Logger<MitochondriaPlugin>.Error($"Failed to create instance of {type.Name}");
            }
        }
    }
}