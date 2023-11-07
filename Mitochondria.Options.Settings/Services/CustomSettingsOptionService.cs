using System.Reflection;
using Mitochondria.Core;
using Mitochondria.Core.Api.Options;
using Mitochondria.Core.Api.Services;
using Mitochondria.Core.Framework.Plugin;
using Mitochondria.Core.Framework.Services;
using Mitochondria.Core.Framework.Utilities.Extensions;
using Mitochondria.Options.Settings.Abstractions;
using Mitochondria.Options.Settings.Framework;
using Mitochondria.Options.Settings.Framework.Managers;
using Reactor.Utilities;

namespace Mitochondria.Options.Settings.Services;

[Service]
public class CustomSettingsOptionService : IService
{
    public void OnStart()
    {
        AddOptionsWithAttribute();
    }

    private void AddOptionsWithAttribute()
    {
        var members = PluginManager.Instance.PluginInfos.Values
            .Select(p => p.Instance)
            .Where(o => o != null)
            .SelectMany(o => o.GetType().Assembly.GetTypes())
            .SelectMany(t => t
                .GetProperties(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                .Concat<MemberInfo>(
                    t.GetFields(
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)))
            .Where(p => p.GetCustomAttributes<CustomSettingsOptionAttribute>().Any());

        foreach (var member in  members)
        {
            if (!member.IsStatic())
            {
                Logger<MitochondriaPlugin>.Warning(
                    $"Member with {nameof(CustomSettingsOptionAttribute)} must be static");
            }
            
            var obj = member switch
            {
                PropertyInfo propertyInfo => propertyInfo.GetValue(null),
                FieldInfo fieldInfo => fieldInfo.GetValue(null),
                _ => throw new Exception("Member should be a property or a field")
            };
            
            foreach (var attribute in member.GetCustomAttributes<CustomSettingsOptionAttribute>())
            {
                TryAddSettingsOption(obj, attribute);
            }
        }
    }

    private void TryAddSettingsOption(object? obj, CustomSettingsOptionAttribute attribute)
    {
        ICustomSettingsOption customSettingsOption;
        
        switch (obj)
        {
            case ICustomOption customOption:
            {
                var pluginType = customOption.GetOwner()!.Instance.GetType();
                var valueType = customOption.ValueType;
                
                customSettingsOption = (ICustomSettingsOption) Activator.CreateInstance(
                    typeof(CustomSettingsOption<,>).MakeGenericType(pluginType, valueType),
                    customOption,
                    attribute.GameMode,
                    attribute.Order)!;
                
                break;
            }
            case ICustomSettingsOption option:
            {
                customSettingsOption = option;
                break;
            }
            case null:
            {
                Logger<MitochondriaPlugin>.Error($"Property with {nameof(CustomSettingsOptionAttribute)} was null");
                return;
            }
            default:
            {
                Logger<MitochondriaPlugin>.Error(
                    $"Property with {nameof(CustomSettingsOptionAttribute)} must be a custom option");

                return;
            }
        }
        
        CustomSettingsOptionManager.Instance.Add(customSettingsOption);
    }
}