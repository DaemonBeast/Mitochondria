using AmongUs.GameOptions;
using Mitochondria.Core.Framework.Resources.Sprites;

namespace Mitochondria.Core.Api.Roles;

public interface ICustomRole
{
    public ICustomRoleDescription Description { get; }
    
    public IRoleSide RoleSide { get; }
    
    public Type RoleSideType { get; }

    public SpriteProvider? Icon { get; }
    
    public int MaxAmount { get; }

    public RoleTypes RoleType { get; }
}