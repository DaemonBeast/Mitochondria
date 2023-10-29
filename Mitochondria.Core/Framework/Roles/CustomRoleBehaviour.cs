using Il2CppInterop.Runtime.Attributes;
using Mitochondria.Core.Api.Roles;
using Mitochondria.Core.Framework.Overrides;
using Reactor.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Mitochondria.Core.Framework.Roles;

[RegisterInIl2Cpp]
public class CustomRoleBehaviour : MonoBehaviour
{
    [HideFromIl2Cpp]
    public ICustomRole? CustomRole { get; private set; }

    private IntroCutsceneTeamOverride? _teamOverride;
    private IntroCutsceneRoleColorOverride? _roleColorOverride;

    [HideFromIl2Cpp]
    public void SetRole<TCustomRole>()
        where TCustomRole : ICustomRole
    {
        SetRole(typeof(TCustomRole));
    }

    [HideFromIl2Cpp]
    public void SetRole(Type customRoleType)
    {
        if (!typeof(ICustomRole).IsAssignableFrom(customRoleType))
        {
            throw new ArgumentException($"{customRoleType} is not a role type");
        }
        
        if (!CustomRoleManager.Instance.TryGet(customRoleType, out var customRole))
        {
            Logger<MitochondriaPlugin>.Error("Cannot set CustomRole in CustomRoleBehaviour to unregistered role");

            return;
        }

        CustomRole = customRole;
        
        UpdateOverrides();
    }

    [HideFromIl2Cpp]
    public void ClearRole()
    {
        CustomRole = null;
        
        RemoveOverrides();
    }

    private void OnDestroy()
    {
        RemoveOverrides();
    }

    private void UpdateOverrides()
    {
        RemoveOverrides();
        
        if (CustomRole == null)
        {
            return;
        }

        _teamOverride = new IntroCutsceneTeamOverride(CustomRole.RoleSide.Title, CustomRole.RoleSide.Color);
        OverrideManager.Instance.Add(_teamOverride);

        _roleColorOverride = new IntroCutsceneRoleColorOverride(CustomRole.Description.Color);
        OverrideManager.Instance.Add(_roleColorOverride);
    }

    private void RemoveOverrides()
    {
        OverrideManager.Instance.Remove(_teamOverride);
        OverrideManager.Instance.Remove(_roleColorOverride);
    }
}