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

    public ICustomNumberOption? AmountOption => _amountOption ??= _amountOptionFactory?.Invoke(this);

    public RoleTeamTypes TeamType { get; }

    private readonly Func<IRoleSide, ICustomNumberOption>? _amountOptionFactory;

    private ICustomNumberOption? _amountOption;

    protected RoleSide(Func<IRoleSide, ICustomNumberOption>? amountOptionFactory = null, RoleTeamTypes? teamType = null)
    {
        _amountOptionFactory = amountOptionFactory;
        TeamType = teamType ?? CustomRoleManager.Instance.CreateRoleTeamType();
    }
}