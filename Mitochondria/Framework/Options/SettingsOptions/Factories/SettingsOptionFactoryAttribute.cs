using System.Reflection;
using Mitochondria.Api.Options.SettingsOptions;
using Reactor.Utilities;

namespace Mitochondria.Framework.Options.SettingsOptions.Factories;

[AttributeUsage(AttributeTargets.Class)]
public class SettingsOptionFactoryAttribute : Attribute
{
    public static void Register(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            var attribute = type.GetCustomAttribute<SettingsOptionFactoryAttribute>();
            if (attribute == null)
            {
                continue;
            }

            if (!type.IsAssignableTo(typeof(ISettingsOptionFactory)))
            {
                throw new Exception(
                    $"Type {type.Name} has {nameof(SettingsOptionFactoryAttribute)} but doesn't extend {nameof(ISettingsOptionFactory)}");
            }

            if (Activator.CreateInstance(type, true) is ISettingsOptionFactory factory)
            {
                SettingsOptionManager.Instance.RegisterFactory(factory);
            }
            else
            {
                Logger<MitochondriaPlugin>.Error($"Failed to create instance of {type.Name}");
            }
        }
    }
}