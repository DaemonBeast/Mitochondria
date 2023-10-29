using AmongUs.GameOptions;
using Mitochondria.Core.Framework.Resources.Sprites;
using Mitochondria.Core.Framework.Roles;

namespace Mitochondria.Core.Api.Roles;

public abstract class CustomRole<TRoleSide> : ICustomRole
    where TRoleSide : IRoleSide
{
    public ICustomRoleDescription Description { get; }

    public IRoleSide RoleSide => _roleSide ??= RoleSideManager.Instance.GetOrThrow(RoleSideType);
    
    public Type RoleSideType { get; }

    public SpriteProvider? Icon { get; }

    public int MaxAmount { get; }

    public RoleTypes RoleType { get; }

    private IRoleSide? _roleSide;

    protected CustomRole(ICustomRoleDescription description, SpriteProvider? icon = null, int maxAmount = int.MaxValue)
    {
        Description = description;
        RoleSideType = typeof(TRoleSide);
        Icon = icon;
        MaxAmount = maxAmount;

        RoleType = CustomRoleManager.Instance.CreateRoleType();
    }
}