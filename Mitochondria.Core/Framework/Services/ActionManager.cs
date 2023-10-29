using System.Reflection;
using Mitochondria.Core.Framework.Utilities;
using Reactor.Utilities;

namespace Mitochondria.Core.Framework.Services;

internal class ActionManager<T1, T2> : AbstractActionManager<Action<T1, T2>>
{
    public static ActionManager<T1, T2> Instance => Singleton<ActionManager<T1, T2>>.Instance;

    public void Invoke(string actionName, T1 arg1, T2 arg2)
    {
        if (Actions.TryGetValue(actionName, out var action))
        {
            foreach (var innerAction in action.GetInvocationList())
            {
                try
                {
                    ((Action<T1, T2>) innerAction).Invoke(arg1, arg2);
                }
                catch (Exception e)
                {
                    Logger<MitochondriaPlugin>.Error($"An exception was thrown by a service: {e}");
                }
            }
        }
    }
}

internal class ActionManager<T1> : AbstractActionManager<Action<T1>>
{
    public static ActionManager<T1> Instance => Singleton<ActionManager<T1>>.Instance;

    public void Invoke(string actionName, T1 arg1)
    {
        if (Actions.TryGetValue(actionName, out var action))
        {
            foreach (var innerAction in action.GetInvocationList())
            {
                try
                {
                    ((Action<T1>) innerAction).Invoke(arg1);
                }
                catch (Exception e)
                {
                    Logger<MitochondriaPlugin>.Error($"An exception was thrown by a service: {e}");
                }
            }
        }
    }
}

internal class ActionManager : AbstractActionManager<Action>
{
    public static ActionManager Instance => Singleton<ActionManager>.Instance;

    public void Invoke(string actionName)
    {
        if (Actions.TryGetValue(actionName, out var action))
        {
            foreach (var innerAction in action.GetInvocationList())
            {
                try
                {
                    ((Action) innerAction).Invoke();
                }
                catch (Exception e)
                {
                    Logger<MitochondriaPlugin>.Error($"An exception was thrown by a service: {e}");
                }
            }
        }
    }
}

internal abstract class AbstractActionManager<T>
    where T : Delegate
{
    public static AbstractActionManager<T>? ActionManager
    {
        get
        {
            if (_actionManager != null)
            {
                return _actionManager;
            }

            var types = typeof(MitochondriaPlugin)
                .Assembly
                .GetTypes();

            var instanceProp = types
                .FirstOrDefault(t => t != typeof(AbstractActionManager<T>) &&
                                     typeof(AbstractActionManager<T>).IsAssignableFrom(t))
                ?.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);

            var genericType = typeof(AbstractActionManager<T>).GetGenericTypeDefinition();
            var actionTypesLength = typeof(T).GetGenericArguments().Length;

            if (instanceProp == null)
            {
                instanceProp = types
                    .FirstOrDefault(
                        t => t.BaseType is { IsGenericType: true } &&
                             t.BaseType.GetGenericTypeDefinition() == genericType &&
                             t.GetGenericArguments().Length == actionTypesLength)
                    ?.MakeGenericType(typeof(T).GetGenericArguments())
                    ?.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            }

            return _actionManager = instanceProp?.GetValue(null) as AbstractActionManager<T>;
        }
    }

    private static AbstractActionManager<T>? _actionManager;

    public Type DelegateType { get; }

    protected readonly Dictionary<string, T> Actions;

    protected AbstractActionManager()
    {
        DelegateType = typeof(T);
        Actions = new Dictionary<string, T>();
    }

    public void Add(string actionName, T newAction)
    {
        Actions[actionName] = Actions.TryGetValue(actionName, out var action)
            ? (T) Delegate.Combine(action, newAction)
            : newAction;
    }

    public bool TryAddBoxed(string actionName, Delegate boxedAction)
    {
        if (boxedAction is not T action)
        {
            Logger<MitochondriaPlugin>.Error("The supplied action is not of the correct type");
            return false;
        }

        Add(actionName, action);
        return true;
    }
}