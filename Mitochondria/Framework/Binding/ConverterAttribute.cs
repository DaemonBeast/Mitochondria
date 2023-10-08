using System.Reflection;
using Mitochondria.Api.Binding;
using Mitochondria.Framework.Utilities;

namespace Mitochondria.Framework.Binding;

[AttributeUsage(AttributeTargets.Class)]
public class ConverterAttribute : Attribute
{
    public static void Register(Assembly assembly)
    {
        TypeUtils.RegisterAttribute<ConverterAttribute, IConverter>(
            assembly,
            type => Converter.Instance.Register(type));
    }
}