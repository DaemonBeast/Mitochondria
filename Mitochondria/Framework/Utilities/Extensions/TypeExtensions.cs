﻿using System.Reflection;
using Il2CppInterop.Runtime;

namespace Mitochondria.Framework.Utilities.Extensions;

public static class TypeExtensions
{
    public static bool IsStatic(this MemberInfo memberInfo)
        => memberInfo switch
        {
            EventInfo eventInfo => eventInfo.IsStatic(),
            FieldInfo fieldInfo => fieldInfo.IsStatic,
            MethodBase methodBase => methodBase.IsStatic,
            PropertyInfo propertyInfo => propertyInfo.IsStatic(),
            _ => throw new ArgumentException(
                $"Identifying if member of type {memberInfo.GetType()} is static is not supported")
        };

    public static bool IsStatic(this EventInfo eventInfo)
        => eventInfo.GetAddMethod()?.IsStatic ?? false;
    
    public static bool IsStatic(this PropertyInfo propertyInfo)
        => propertyInfo.GetAccessors(true).Any(m => m.IsStatic);

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

    public static IEnumerable<Type> GetParents(this Type type, bool includeSelf = false, bool includeInterfaces = false)
    {
        if (includeSelf)
        {
            yield return type;
        }

        var currentType = type.BaseType;
        var parentType = currentType?.BaseType;

        while (parentType != null)
        {
            yield return currentType!;
            currentType = parentType;
            parentType = parentType.BaseType;
        }

        if (includeInterfaces)
        {
            foreach (var typeInterface in type.GetInterfaces())
            {
                yield return typeInterface;
            }
        }
    }
}