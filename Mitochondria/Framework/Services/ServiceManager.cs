using System.Collections.Immutable;
using System.Reflection;
using BepInEx;
using Il2CppInterop.Generator.Extensions;
using Mitochondria.Api.Services;
using Mitochondria.Framework.Utilities;
using Mitochondria.Framework.Utilities.Extensions;
using Reactor.Utilities;

namespace Mitochondria.Framework.Services;

public class ServiceManager
{
    public static ServiceManager Instance => Singleton<ServiceManager>.Instance;

    public IReadOnlyDictionary<Type, IService> Services => _services.ToImmutableDictionary();

    private readonly Dictionary<Type, IService> _services;
    private readonly Dictionary<string, List<Action<object?[]?>>> _serviceMethods;

    private ServiceManager()
    {
        _services = new Dictionary<Type, IService>();
        _serviceMethods = new Dictionary<string, List<Action<object?[]?>>>();
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

        var serviceNamespace = typeof(IService).FullName!;

        var methods = serviceType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.Name.StartsWith(serviceNamespace) && service.GetType().Implements(typeof(IService), m));

        foreach (var method in methods)
        {
            _serviceMethods
                .GetOrCreate(method.Name[(serviceNamespace.Length + 1)..], _ => new List<Action<object?[]?>>())
                .Add(args => method.Invoke(service, args));
        }
    }

    internal bool TryInvokeMethod(string methodName, params object?[]? args)
    {
        if (!_serviceMethods.TryGetValue(methodName, out var actions))
        {
            // ignore unknown method names
            return true;
        }

        foreach (var action in actions)
        {
            try
            {
                action.Invoke(args);
            }
            catch (MemberAccessException)
            {
                // (probably) invalid args
                return false;
            }
            catch (Exception e)
            {
                Logger<MitochondriaPlugin>.Error($"An exception was thrown by a service: {e}");
            }
        }

        return true;
    }
}