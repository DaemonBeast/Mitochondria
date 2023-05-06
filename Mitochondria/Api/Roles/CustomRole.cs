using AmongUs.GameOptions;
using Mitochondria.Framework.Resources.Sprites;
using Mitochondria.Framework.Roles;

namespace Mitochondria.Api.Roles;

public abstract class CustomRole<TRoleSide> : ICustomRole
    where TRoleSide : IRoleSide
{
    public ICustomRoleDescription Description { get; }

    public IRoleSide RoleSide => _roleSide ??= RoleSideManager.Instance.GetOrThrow(RoleSideType);
    
    public Type RoleSideType { get; }

    public SpriteReference? Icon { get; }

    public int MaxAmount { get; }

    public RoleTypes RoleType { get; }

    private IRoleSide? _roleSide;

    protected CustomRole(ICustomRoleDescription description, SpriteReference? icon = null, int maxAmount = int.MaxValue)
    {
        Description = description;
        RoleSideType = typeof(TRoleSide);
        Icon = icon;
        MaxAmount = maxAmount;

        RoleType = CustomRoleManager.Instance.CreateRoleType();
    }
}