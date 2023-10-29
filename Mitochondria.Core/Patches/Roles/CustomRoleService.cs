using System.Collections.Immutable;
using AmongUs.GameOptions;
using Mitochondria.Core.Api.Roles;
using Mitochondria.Core.Api.Services;
using Mitochondria.Core.Framework.Roles;
using Mitochondria.Core.Framework.Services;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Core.Patches.Roles;

[Service]
public class CustomRoleService : IService
{
    public ImmutableDictionary<ICustomRole, GameObject> RoleObjects => _roleObjects.ToImmutableDictionary();

    public ImmutableDictionary<ICustomRole, RoleBehaviour> RoleBehaviours => _roleBehaviours.ToImmutableDictionary();

    private readonly Dictionary<ICustomRole, GameObject> _roleObjects;
    private readonly Dictionary<ICustomRole, RoleBehaviour> _roleBehaviours;

    private bool _addedRoles;

    private CustomRoleService()
    {
        _roleObjects = new Dictionary<ICustomRole, GameObject>();
        _roleBehaviours = new Dictionary<ICustomRole, RoleBehaviour>();
        
        CustomRoleManager.Instance.OnCustomRoleRegistered += CustomRoleRegistered;
    }

    public void OnHudStart(HudManager hudManager)
    {
        if (_addedRoles)
        {
            return;
        }

        _addedRoles = true;

        var roleBehaviours = _roleBehaviours.Values;

        var allRoles = RoleManager.Instance.AllRoles;
        var fontMaterial = allRoles.First(r => r.Ability != null).Ability.FontMaterial;
        var introSound = allRoles.First(r => r.Role == RoleTypes.Crewmate).IntroSound;
        
        foreach (var roleBehaviour in roleBehaviours)
        {
            roleBehaviour.Ability.FontMaterial = fontMaterial;

            if (roleBehaviour.IntroSound == null)
            {
                roleBehaviour.IntroSound = introSound;
            }
        }

        RoleManager.Instance.AllRoles = RoleManager.Instance.AllRoles.Concat(_roleBehaviours.Values).ToArray();
    }

    private GameObject CreateRoleObject(ICustomRole customRole)
    {
        var roleTitle = customRole.Description.Title;

        var roleObject = new GameObject(roleTitle).DontDestroy();
        _roleObjects.Add(customRole, roleObject);

        return roleObject;
    }

    private RoleBehaviour CreateRoleBehaviour(ICustomRole customRole, GameObject roleObject)
    {
        var customRoleDescription = customRole.Description;
        
        var abilityButtonSettings = ScriptableObject.CreateInstance<AbilityButtonSettings>();
        abilityButtonSettings.Image = (customRole.Icon ?? Assets.Sprites.Empty).Load();
        abilityButtonSettings.Text = customRole.Description.TitleName;

        var roleBehaviour = roleObject.AddComponent<CrewmateRole>();
        roleBehaviour.Ability = abilityButtonSettings;
        
        roleBehaviour.StringName = customRoleDescription.TitleName;
        roleBehaviour.BlurbName = customRoleDescription.CutsceneDescriptionName;
        roleBehaviour.BlurbNameLong = customRoleDescription.RoleMenuDescriptionName;
        roleBehaviour.BlurbNameMed = customRoleDescription.TaskListDescriptionName;
        
        roleBehaviour.NameColor = customRole.Description.Color;
        
        // TODO: Defaults to crewmate sound. Should be made configurable when audio file loading is added
        // roleBehaviour.IntroSound = ;
        
        roleBehaviour.Role = customRole.RoleType;
        roleBehaviour.TeamType = customRole.RoleSide.TeamType;
        roleBehaviour.MaxCount = customRole.MaxAmount;
        
        roleBehaviour.CanVent = true;
        roleBehaviour.CanUseKillButton = true;
        roleBehaviour.CanBeKilled = true;
        
        _roleBehaviours.Add(customRole, roleBehaviour);

        return roleBehaviour;
    }

    private void CustomRoleRegistered(ICustomRole customRole)
    {
        CreateRoleBehaviour(customRole, CreateRoleObject(customRole));
    }
}