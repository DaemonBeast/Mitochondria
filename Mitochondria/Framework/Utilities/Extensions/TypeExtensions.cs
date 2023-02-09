using System.Reflection;

namespace Mitochondria.Framework.Utilities.Extensions;

public static class TypeExtensions
{
    public static bool IsStatic(this PropertyInfo propertyInfo, bool nonPublic = false)
        => propertyInfo.GetAccessors(nonPublic).Any(m => m.IsStatic);

    public static bool Implements(this Type type, Type interfaceType, string methodName)
    {
        var methodInfo = type.GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        return methodInfo != null && type.Implements(interfaceType, methodInfo);
    }

    public static bool Implements(this Type type, Type interfaceType, MethodInfo methodInfo)
    {
        return type.GetInterfaceMap(interfaceType).TargetMethods.Contains(methodInfo);
    }
}