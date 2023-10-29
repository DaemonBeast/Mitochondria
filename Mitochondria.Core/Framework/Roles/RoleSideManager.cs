using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Mitochondria.Core.Api.Roles;
using Mitochondria.Core.Framework.Utilities;

namespace Mitochondria.Core.Framework.Roles;

public class RoleSideManager
{
    public static RoleSideManager Instance => Singleton<RoleSideManager>.Instance;

    public ImmutableDictionary<Type, IRoleSide> RoleSides => _roleSides.ToImmutableDictionary();

    private readonly Dictionary<Type, IRoleSide> _roleSides;

    private RoleSideManager()
    {
        _roleSides = new Dictionary<Type, IRoleSide>();
    }
    
    public TRoleSide Register<TRoleSide>()
        where TRoleSide : class, IRoleSide
    {
        return (TRoleSide) Register(typeof(TRoleSide));
    }

    public IRoleSide Register(Type roleSideType)
    {
        if (!typeof(IRoleSide).IsAssignableFrom(roleSideType))
        {
            throw new ArgumentException($"{roleSideType} does not inherit from {nameof(IRoleSide)}");
        }

        if (_roleSides.TryGetValue(roleSideType, out var roleSide))
        {
            return roleSide;
        }
        
        roleSide = (IRoleSide) Activator.CreateInstance(roleSideType, true)!;
        _roleSides[roleSideType] = roleSide;

        return roleSide;
    }

    public bool TryGet<TRoleSide>([NotNullWhen(true)] out TRoleSide? roleSide)
        where TRoleSide : class, IRoleSide
    {
        var result = TryGet(typeof(TRoleSide), out var r);

        roleSide = result ? (TRoleSide) r! : null;
        return result;
    }

    public bool TryGet(Type roleSideType, [NotNullWhen(true)] out IRoleSide? roleSide)
        => _roleSides.TryGetValue(roleSideType, out roleSide);

    public TRoleSide GetOrThrow<TRoleSide>()
        where TRoleSide : class, IRoleSide
    {
        return (TRoleSide) GetOrThrow(typeof(TRoleSide));
    }

    public IRoleSide GetOrThrow(Type roleSideType)
    {
        return _roleSides.GetValueOrDefault(roleSideType) ??
               throw new Exception($"Failed to get {roleSideType}");
    }
    
    public bool TryGetRoleSide(RoleTeamTypes teamType, [NotNullWhen(true)] out IRoleSide? roleSide)
    {
        return (roleSide = _roleSides.Values.FirstOrDefault(r => r.TeamType == teamType)) != null;
    }
}