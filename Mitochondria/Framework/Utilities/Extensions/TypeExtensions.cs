using System.Reflection;

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

    public static Type ToActionType(this Type[] types)
    {
        if (types.Length == 0)
        {
            return typeof(Action);
        }

        return (types.Length switch
        {
            1 => typeof(Action<>),
            2 => typeof(Action<,>),
            3 => typeof(Action<,,>),
            4 => typeof(Action<,,,>),
            5 => typeof(Action<,,,,>),
            6 => typeof(Action<,,,,,>),
            7 => typeof(Action<,,,,,,>),
            8 => typeof(Action<,,,,,,,>),
            9 => typeof(Action<,,,,,,,,>),
            10 => typeof(Action<,,,,,,,,,>),
            11 => typeof(Action<,,,,,,,,,,>),
            12 => typeof(Action<,,,,,,,,,,,>),
            13 => typeof(Action<,,,,,,,,,,,,>),
            14 => typeof(Action<,,,,,,,,,,,,,>),
            15 => typeof(Action<,,,,,,,,,,,,,,>),
            16 => typeof(Action<,,,,,,,,,,,,,,,>),
            _ => throw new ArgumentOutOfRangeException(
                nameof(types),
                "Cannot create action type with more than 16 types")
        }).MakeGenericType(types);
    }
}