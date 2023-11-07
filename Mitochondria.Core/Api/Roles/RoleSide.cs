using Mitochondria.Core.Framework.Options;
using Mitochondria.Core.Framework.Roles;
using UnityEngine;

namespace Mitochondria.Core.Api.Roles;

public abstract class RoleSide : IRoleSide
{
    public abstract string Title { get; }

    public virtual string PluralTitle => $"{Title}s";

    public abstract Color Color { get; }

    public abstract bool IsImpostor { get; }

    public ICustomNumberOption? AmountOption { get; }

    public RoleTeamTypes TeamType { get; }

    protected RoleSide(Func<IRoleSide, ICustomNumberOption>? amountOptionFactory = null)
    {
        TeamType = CustomRoleManager.Instance.CreateRoleTeamType();
        AmountOption = amountOptionFactory?.Invoke(this);
    }

    protected internal RoleSide(
        RoleTeamTypes teamType,
        Func<IRoleSide, ICustomNumberOption>? amountOptionFactory = null)
    {
        TeamType = teamType;
        AmountOption = amountOptionFactory?.Invoke(this);
    }
}