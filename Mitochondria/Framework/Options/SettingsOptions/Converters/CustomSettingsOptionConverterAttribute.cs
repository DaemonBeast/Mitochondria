using System.Reflection;
using Mitochondria.Api.Options.SettingsOptions;
using Reactor.Utilities;

namespace Mitochondria.Framework.Options.SettingsOptions.Converters;

[AttributeUsage(AttributeTargets.Class)]
public class CustomSettingsOptionConverterAttribute : Attribute
{
    public static void Register(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            var attribute = type.GetCustomAttribute<CustomSettingsOptionConverterAttribute>();
            if (attribute == null)
            {
                continue;
            }

            if (!type.IsAssignableTo(typeof(ICustomSettingsOptionConverter)))
            {
                throw new Exception(
                    $"Type {type.Name} has {nameof(CustomSettingsOptionConverterAttribute)} but doesn't extend {nameof(ICustomSettingsOptionConverter)}");
            }

            if (Activator.CreateInstance(type, true) is ICustomSettingsOptionConverter converter)
            {
                CustomSettingsOptionManager.Instance.RegisterConverter(converter);
            }
            else
            {
                Logger<MitochondriaPlugin>.Error($"Failed to create instance of {type.Name}");
            }
        }
    }
}