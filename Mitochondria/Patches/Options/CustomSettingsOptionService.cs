using System.Reflection;
using Mitochondria.Api.Options;
using Mitochondria.Api.Options.SettingsOptions;
using Mitochondria.Api.Services;
using Mitochondria.Framework.Options.SettingsOptions;
using Mitochondria.Framework.Options.SettingsOptions.Managers;
using Mitochondria.Framework.Plugin;
using Mitochondria.Framework.Services;
using Mitochondria.Framework.Utilities.Extensions;
using Reactor.Utilities;

namespace Mitochondria.Patches.Options;

[Service]
public class CustomSettingsOptionService : IService
{
    void IService.OnStart()
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
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Concat<MemberInfo>(t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)))
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