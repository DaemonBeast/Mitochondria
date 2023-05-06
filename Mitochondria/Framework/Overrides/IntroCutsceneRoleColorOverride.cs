using Mitochondria.Api.Overrides;
using UnityEngine;

namespace Mitochondria.Framework.Overrides;

public class IntroCutsceneRoleColorOverride : IOverride<IntroCutsceneRoleColorOverride>
{
    public Color? Color { get; }

    public int Priority { get; }

    public IntroCutsceneRoleColorOverride(Color? color = null, int priority = 0)
    {
        Color = color;
        Priority = priority;
    }

    public IntroCutsceneRoleColorOverride Override(IntroCutsceneRoleColorOverride otherOverride)
        => new(Color ?? otherOverride.Color, Priority);
}