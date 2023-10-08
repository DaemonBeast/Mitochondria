using System.Diagnostics.CodeAnalysis;
using Il2CppInterop.Generator.Extensions;
using Mitochondria.Framework.Utilities.Extensions;

namespace Mitochondria.Framework.Utilities;

public class TypeGraph<T>
{
    public static TypeGraph<T> Shared { get; }

    public List<T> Values { get; }

    public Dictionary<Type, TypeGraph<T>> Children { get; }

    private readonly Dictionary<Type, TypeGraph<T>> _lookupTable;

    static TypeGraph()
    {
        Shared = new TypeGraph<T>();
    }

    public TypeGraph()
    {
        Values = new List<T>();
        Children = new Dictionary<Type, TypeGraph<T>>();
        _lookupTable = new Dictionary<Type, TypeGraph<T>>();
    }

    public void Add(Type type, T value)
    {
        if (_lookupTable.TryGetValue(type, out var typeGraph))
        {
            typeGraph.Values.Add(value);
            return;
        }

        if (type.IsInterface)
        {
            GetOrCreateBaseNode(type).Values.Add(value);
            return;
        }

        TypeGraph<T>? currentNode = null;

        var parentTypes = type
            .GetParents()
            .TakeWhile(parentType =>
            {
                if (_lookupTable.TryGetValue(parentType, out typeGraph))
                {
                    currentNode = typeGraph;
                    return false;
                }

                return true;
            })
            .ToArray();

        var iterStart = parentTypes.Length - 1;

        if (currentNode == null)
        {
            var interfaces = type.GetInterfaces();
            var baseParentType = parentTypes[iterStart--];

            if (interfaces.Length == 0)
            {
                currentNode = GetOrCreateBaseNode(baseParentType);
            }
            else
            {
                _lookupTable.Add(baseParentType, currentNode = new TypeGraph<T>());

                foreach (var interfaceType in interfaces)
                {
                    GetOrCreateBaseNode(interfaceType).Children.Add(type, currentNode);
                }
            }
        }

        for (; iterStart >= 0; iterStart--)
        {
            var parentType = parentTypes[iterStart];

            _lookupTable.Add(
                parentType,
                currentNode = currentNode.Children.GetOrCreate(parentType, _ => new TypeGraph<T>()));
        }

        _lookupTable.Add(type, currentNode.Children[type] = currentNode = new TypeGraph<T>());
        currentNode.Values.Add(value);
    }

    public bool TryGet(Type type, [NotNullWhen(true)] out TypeGraph<T>? typeGraph)
        => _lookupTable.TryGetValue(type, out typeGraph);

    public IEnumerable<T> Traverse()
        => Children.Values.Aggregate(Values.AsEnumerable(), (current, child) => current.Concat(child.Traverse()));

    private TypeGraph<T> GetOrCreateBaseNode(Type type)
    {
        if (Children.TryGetValue(type, out var typeGraph))
        {
            return typeGraph;
        }

        _lookupTable.Add(type, Children[type] = typeGraph = new TypeGraph<T>());
        return typeGraph;
    }
}