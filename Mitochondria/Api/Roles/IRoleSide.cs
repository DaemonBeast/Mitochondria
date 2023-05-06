using Mitochondria.Framework.Options;
using UnityEngine;

namespace Mitochondria.Api.Roles;

public interface IRoleSide
{
    public string Title { get; }
    
    public string PluralTitle { get; }
    
    public Color Color { get; }
    
    public bool IsImpostor { get; }

    public ICustomNumberOption? AmountOption { get; }

    public RoleTeamTypes TeamType { get; }
}