using System.Reflection;
using Mitochondria.Api.Options.SettingsOptions;
using Reactor.Utilities;

namespace Mitochondria.Framework.Options.SettingsOptions.Providers;

[AttributeUsage(AttributeTargets.Class)]
public class SettingsOptionProviderAttribute : Attribute
{
    public static void Register(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            var attribute = type.GetCustomAttribute<SettingsOptionProviderAttribute>();
            if (attribute == null)
            {
                continue;
            }

            if (!type.IsAssignableTo(typeof(ISettingsOptionProvider)))
            {
                throw new Exception(
                    $"Type {type.Name} has {nameof(SettingsOptionProviderAttribute)} but doesn't extend {nameof(ISettingsOptionProvider)}");
            }

            if (Activator.CreateInstance(type, true) is ISettingsOptionProvider provider)
            {
                SettingsOptionManager.Instance.RegisterProvider(provider);
            }
            else
            {
                Logger<MitochondriaPlugin>.Error($"Failed to create instance of {type.Name}");
            }
        }
    }
}