using System.Collections.Immutable;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using Mitochondria.Core.Api.Services;
using Mitochondria.Core.Framework.Plugin.Extensions;
using Mitochondria.Core.Framework.Utilities;
using Mitochondria.Core.Framework.Utilities.Extensions;

namespace Mitochondria.Core.Framework.Services;

public class ServiceManager
{
    public static ServiceManager Instance => Singleton<ServiceManager>.Instance;

    public IReadOnlyDictionary<Type, IService> Services => _services.ToImmutableDictionary();

    private readonly Dictionary<Type, IService> _services;

    private ServiceManager()
    {
        _services = new Dictionary<Type, IService>();
    }

    public void Register(Type serviceType, PluginInfo pluginInfo)
    {
        if (_services.ContainsKey(serviceType))
        {
            return;
        }

        var service = Activator.CreateInstance(serviceType, true) as IService ??
                      throw new Exception($"Cannot register {serviceType.Name} as a service");

        service.SetOwner(pluginInfo);
        _services[serviceType] = service;

        var methods = serviceType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => service.GetType().Implements(typeof(IService), m));

        foreach (var method in methods)
        {
            var methodName = method.Name[(method.Name.LastIndexOf('.') + 1)..];
            var actionType = method.GetParameters().Types().ToActionType();
            var action = method.CreateDelegate(actionType, service);

            var actionManager = typeof(AbstractActionManager<>)
                .MakeGenericType(actionType)
                .GetProperty(
                    nameof(AbstractActionManager<Delegate>.ActionManager), BindingFlags.Public | BindingFlags.Static)
                !.GetValue(null)!;

            actionManager
                .GetType()
                .GetMethod(nameof(AbstractActionManager<Delegate>.TryAddBoxed))
                !.Invoke(actionManager, new object?[] { methodName, action });
        }
    }
}