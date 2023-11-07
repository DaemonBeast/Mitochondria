using System.Reflection;
using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Framework.Utilities;

namespace Mitochondria.Core.Framework.Binding;

[AttributeUsage(AttributeTargets.Class)]
public class BindingAttribute : Attribute
{
    public static void Register(Assembly assembly)
    {
        TypeUtils.RegisterAttribute<BindingAttribute, IBinding>(
            assembly,
            type => BindingManager.Instance.Register(type));
    }
}