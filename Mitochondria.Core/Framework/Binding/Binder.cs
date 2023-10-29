using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Il2CppInterop.Generator.Extensions;
using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Framework.Utilities;
using Mitochondria.Core.Framework.Utilities.Extensions;
using Reactor.Utilities;
using UnityEngine;

namespace Mitochondria.Core.Framework.Binding;

public class Binder
{
    public static Binder Instance => Singleton<Binder>.Instance;

    public ImmutableArray<IBinding> Bindings => _bindings.Immutable;

    public Dictionary<Type, Dictionary<Type, Type>> BindingTypes { get; }

    private readonly ImmutableArrayWrapper<IBinding> _bindings;

    private Binder()
    {
        BindingTypes = new Dictionary<Type, Dictionary<Type, Type>>();
        _bindings = new ImmutableArrayWrapper<IBinding>();
    }

    public bool Remove(IBinding binding)
    {
        var removed = _bindings.Remove(binding);
        if (removed)
        {
            binding.Dispose();
        }

        return removed;
    }

    public void Register<TBinding>()
        => Register(typeof(TBinding));

    public void Register(Type bindingType)
    {
        var bindingGenericType = typeof(IBinding<,>);

        var objTypes = bindingType
            .GetInterfaces()
            .FirstOrDefault(i => i.GetGenericTypeDefinition() == bindingGenericType)
            ?.GetGenericArguments();

        if (objTypes == null)
        {
            Logger<MitochondriaPlugin>.Error(
                $"Cannot register {bindingType.FullName} as a binding as it does not inherit {bindingGenericType.FullName}");

            return;
        }

        BindingTypes.GetOrCreate(objTypes[0], _ => new Dictionary<Type, Type>()).Add(objTypes[1], bindingType);
    }

    public bool TryGetBindingType(Type objType, Type otherType, [NotNullWhen(true)] out Type? bindingType)
    {
        if ((BindingTypes.TryGetValue(objType, out var innerTypes) &&
             innerTypes.TryGetValue(otherType, out bindingType)) ||
            (BindingTypes.TryGetValue(otherType, out innerTypes) &&
             innerTypes.TryGetValue(objType, out bindingType)))
        {
            return true;
        }

        var objTypeHierarchy = objType.GetParents(true, true).ToArray();
        var otherTypeHierarchy = otherType.GetParents(true, true).ToArray();

        foreach (var combination in objTypeHierarchy.Combine(otherTypeHierarchy))
        {
            if ((BindingTypes.TryGetValue(combination.Item1, out innerTypes) &&
                 innerTypes.TryGetValue(combination.Item2, out bindingType)) ||
                (BindingTypes.TryGetValue(combination.Item2, out innerTypes) &&
                 innerTypes.TryGetValue(combination.Item1, out bindingType)))
            {
                return true;
            }
        }

        bindingType = null;
        return false;
    }

    public bool TryBind(object obj, object other)
    {
        var objType = obj.GetType();
        var otherType = other.GetType();

        if (TryGetBindingType(objType, otherType, out var bindingType))
        {
            var instance = (IBinding) Activator.CreateInstance(bindingType, true)!;
            instance.BoxedObj = obj;
            instance.BoxedOther = other;
            instance.BindEvents();

            if (obj.Is<GameObject>(out var gameObject))
            {
                gameObject.AddComponent<BindingBehaviour>().Binding = instance;
            }
            else if (obj.Is<MonoBehaviour>(out var monoBehaviour))
            {
                monoBehaviour.gameObject.AddComponent<BindingBehaviour>().Binding = instance;
            }

            if (other.Is<GameObject>(out gameObject))
            {
                gameObject.AddComponent<BindingBehaviour>().Binding = instance;
            }
            else if (other.Is<MonoBehaviour>(out var monoBehaviour))
            {
                monoBehaviour.gameObject.AddComponent<BindingBehaviour>().Binding = instance;
            }

            _bindings.Add(instance);

            return true;
        }

        Logger<MitochondriaPlugin>.Error(
            $"No binding could be found to bind together {objType.FullName} and {otherType.FullName}");

        return false;
    }
}