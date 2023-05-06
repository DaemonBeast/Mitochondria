using Mitochondria.Api.Overrides;
using UnityEngine;

namespace Mitochondria.Framework.Overrides;

public class IntroCutsceneTeamOverride : IOverride<IntroCutsceneTeamOverride>
{
    public string? Title { get; }

    public Color? Color { get; }

    public int Priority { get; }

    public IntroCutsceneTeamOverride(string? title = null, Color? color = null, int priority = 0)
    {
        Title = title;
        Color = color;
        Priority = priority;
    }

    public IntroCutsceneTeamOverride Override(IntroCutsceneTeamOverride otherOverride)
        => new(Title ?? otherOverride.Title, Color ?? otherOverride.Color, Priority);
}