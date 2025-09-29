namespace Mitochondria.GameModes.Utilities.Extensions;

public static class TypeExtensions
{
    public static IEnumerable<Type> GetBaseTypes(this Type type)
    {
        var baseType = type.BaseType;

        while (baseType != null)
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }
    }
}
