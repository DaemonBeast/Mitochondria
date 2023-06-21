using System.Reflection;
using Mitochondria.Api.Roles;
using Mitochondria.Framework.Utilities;

namespace Mitochondria.Framework.Roles;

[AttributeUsage(AttributeTargets.Class)]
public class CustomRoleAttribute : Attribute
{
    public static void Register(Assembly assembly)
    {
        TypeUtils.RegisterAttribute<CustomRoleAttribute, ICustomRole>(
            assembly,
            type => CustomRoleManager.Instance.Register(type));
    }
}