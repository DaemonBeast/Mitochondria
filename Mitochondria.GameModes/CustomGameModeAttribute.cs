using System.Reflection;
using Mitochondria.GameModes.Utilities.Extensions;

namespace Mitochondria.GameModes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CustomGameModeAttribute : Attribute
{
    public static void Register(Assembly assembly)
    {
        var baseGenericType = typeof(BaseCustomGameMode<>);

        foreach (var type in assembly.GetTypes())
        {
            if (type.GetCustomAttribute<CustomGameModeAttribute>() == null) continue;

            var baseType = type.GetBaseTypes()
                .FirstOrDefault(potentialType => potentialType.IsGenericType &&
                                                 potentialType.GetGenericTypeDefinition() == baseGenericType);

            if (baseType == null)
            {
                Warning(
                    $"Type \"{type.Name}\" has \"{nameof(CustomGameModeAttribute)}\" but does not implement \"{baseGenericType.Name}\".");

                continue;
            }

            CustomGameModeManager.Register(type);
        }
    }
}
