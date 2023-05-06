using System.Reflection;
using Mitochondria.Api.Roles;
using Mitochondria.Framework.Utilities;

namespace Mitochondria.Framework.Roles;

[AttributeUsage(AttributeTargets.Class)]
public class RegisterCustomRoleAttribute : Attribute
{
    public static void Register(Assembly assembly)
    {
        TypeUtilities.RegisterAttribute<RegisterCustomRoleAttribute, ICustomRole>(
            assembly,
            type => CustomRoleManager.Instance.Register(type));
    }
}