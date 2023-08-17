using System.Reflection;
using Mitochondria.Api.Options.SettingsOptions;
using Mitochondria.Framework.Options.SettingsOptions.Managers;
using Mitochondria.Framework.Utilities;

namespace Mitochondria.Framework.Options.SettingsOptions.Converters;

[AttributeUsage(AttributeTargets.Class)]
public class SettingsOptionConverterAttribute : Attribute
{
    public static void Register(Assembly assembly)
    {
        TypeUtils.RegisterAttribute<SettingsOptionConverterAttribute, ISettingsOptionConverter>(
            assembly,
            type => SettingsOptionConverterManager.Instance.Register(type));
    }
}