using System.Reflection;
using Reactor.Utilities;

namespace Mitochondria.Framework.Utilities;

public static class TypeUtils
{
    public static void RegisterAttribute<TAttribute, TInherits>(
        Assembly assembly,
        Action<Type> callback,
        Action<Type>? invalidTypeCallback = null)
        where TAttribute : Attribute
        where TInherits : class
    {
        var invalidCallback = invalidTypeCallback ??
                              (type => Logger<MitochondriaPlugin>.Error(
                                  $"Type {type.Name} has {typeof(TAttribute).Name} but doesn't extend {typeof(TInherits).FullName}"));
        
        foreach (var type in assembly.GetTypes())
        {
            var attribute = type.GetCustomAttribute<TAttribute>();
            if (attribute == null)
            {
                continue;
            }

            if (!type.IsAssignableTo(typeof(TInherits)))
            {
                invalidCallback.Invoke(type);
                continue;
            }
            
            callback.Invoke(type);
        }
    }
}