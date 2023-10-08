using System.Reflection;
using Mitochondria.Api.Binding;
using Mitochondria.Framework.Utilities;

namespace Mitochondria.Framework.Binding;

[AttributeUsage(AttributeTargets.Class)]
public class BindingAttribute : Attribute
{
    public static void Register(Assembly assembly)
    {
        TypeUtils.RegisterAttribute<BindingAttribute, IBinding>(
            assembly,
            type => Binder.Instance.Register(type));
    }
}