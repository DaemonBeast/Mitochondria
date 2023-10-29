using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using AmongUs.GameOptions;
using Mitochondria.Core.Api.Roles;
using Mitochondria.Core.Framework.Utilities;

namespace Mitochondria.Core.Framework.Roles;

public class CustomRoleManager
{
    public static CustomRoleManager Instance => Singleton<CustomRoleManager>.Instance;

    public ImmutableDictionary<Type, ICustomRole> CustomRoles => _customRoles.ToImmutableDictionary();

    public delegate void CustomRoleRegisteredHandler(ICustomRole customRole);

    public event CustomRoleRegisteredHandler? OnCustomRoleRegistered;

    private readonly Dictionary<Type, ICustomRole> _customRoles;
    private ushort _currentRoleId = ushort.MaxValue;
    private ushort _currentRoleTeamId = ushort.MaxValue;

    private CustomRoleManager()
    {
        _customRoles = new Dictionary<Type, ICustomRole>();
    }

    public void Register<TCustomRole>()
        where TCustomRole : ICustomRole
    {
        Register(typeof(TCustomRole));
    }

    public void Register(Type customRoleType)
    {
        if (!typeof(ICustomRole).IsAssignableFrom(customRoleType))
        {
            throw new ArgumentException($"{customRoleType} is not a role type");
        }

        if (_customRoles.ContainsKey(customRoleType))
        {
            return;
        }

        var customRole = (ICustomRole) Activator.CreateInstance(customRoleType)!;
        _customRoles.Add(customRoleType, customRole);
        RoleSideManager.Instance.Register(customRole.RoleSideType);
        
        OnCustomRoleRegistered?.Invoke(customRole);
    }

    public bool TryGet<TCustomRole>([NotNullWhen(true)] out TCustomRole? customRole)
        where TCustomRole : ICustomRole
    {
        var result = TryGet(typeof(TCustomRole), out var c);

        customRole = (TCustomRole) c!;
        return result;
    }

    public bool TryGet(Type customRoleType, [NotNullWhen(true)] out ICustomRole? customRole)
    {
        return _customRoles.TryGetValue(customRoleType, out customRole);
    }
    
    public bool TryGet(RoleTypes roleType, [NotNullWhen(true)] out ICustomRole? customRole)
    {
        return (customRole = _customRoles.Values.FirstOrDefault(c => c.RoleType == roleType)) != null;
    }
    
    public RoleTypes CreateRoleType()
    {
        return (RoleTypes) _currentRoleId--;
    }

    public RoleTeamTypes CreateRoleTeamType()
    {
        return (RoleTeamTypes) _currentRoleTeamId--;
    }
}