using System.Reflection;
using Mitochondria.Core.Api.Roles;
using Mitochondria.Core.Framework.Utilities;

namespace Mitochondria.Core.Framework.Roles;

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