using System.Reflection;
using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Framework.Utilities;

namespace Mitochondria.Core.Framework.Binding;

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