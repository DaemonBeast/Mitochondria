﻿using System.Reflection;
using Reactor.Utilities;

namespace Mitochondria.Framework.Utilities;

public static class TypeUtilities
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
                                  $"Type {type.Name} has {nameof(TAttribute)} but doesn't extend {nameof(TInherits)}"));
        
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
            }
            
            callback.Invoke(type);
        }
    }
}