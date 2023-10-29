using BepInEx.Unity.IL2CPP;
using Mitochondria.Core.Api.Options;
using UnityEngine;

namespace Mitochondria.Core.Framework.Options;

public class CustomNumberOption<TPlugin> : CustomOption<TPlugin, float>, ICustomNumberOption
    where TPlugin : BasePlugin
{
    public override string ValueString =>
        ZeroIsInfinity && Mathf.Approximately(Value, 0f) ? "∞" : string.Format(FormatString, Value);

    public FloatRange Range { get; }
    
    public float Step { get; }

    public bool ZeroIsInfinity { get; }

    public CustomNumberOption(
        string id,
        string title,
        float value,
        FloatRange range,
        float step = 1f,
        string? formatString = null,
        bool zeroIsInfinity = false,
        bool sync = true) : base(id, title, value, formatString, sync)
    {
        Range = range;
        Step = step;
        ZeroIsInfinity = zeroIsInfinity;
    }

    public float Increase(int steps = 1)
    {
        return Value = Range.Clamp(Value + Step * steps);
    }

    public float Decrease(int steps = 1)
    {
        return Value = Range.Clamp(Value - Step * steps);
    }
}

public interface ICustomNumberOption : ICustomOption<float>
{
    public FloatRange Range { get; }
    
    public float Step { get; }

    public bool ZeroIsInfinity { get; }

    public float Increase(int steps = 1);

    public float Decrease(int steps = 1);
}