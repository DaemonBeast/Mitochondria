using BepInEx.Unity.IL2CPP;
using Mitochondria.Api.Options;
using Mitochondria.Framework.Utilities.Extensions;
using Reactor.Localization.Utilities;

namespace Mitochondria.Framework.Options;

public class CustomStringOption<TPlugin, TEnum> : CustomOption<TPlugin, TEnum>, ICustomStringOption<TEnum>
    where TPlugin : BasePlugin
    where TEnum : struct, Enum
{
    public override string ValueString =>
        string.Format(FormatString, TranslationController.Instance.GetString(ValueNames[ValueInt]));

    public int ValueInt
    {
        get => Value.GetIndex();
        set => Value = value.GetEnumFromIndex<TEnum>() ?? (TEnum) (object) 0;
    }

    public StringNames[] ValueNames { get; }

    public CustomStringOption(
        string title,
        TEnum value,
        IDictionary<TEnum, string>? displayValues = null,
        string? formatString = null,
        bool sync = true) : base(title, value, formatString, sync)
    {
        ValueNames = Enum.GetValues<TEnum>()
            .Select(e => CustomStringName.CreateAndRegister(
                displayValues?.TryGetValue(e, out var displayName) ?? false ? displayName : e.ToString()))
            .ToArray();
    }
}

public interface ICustomStringOption<TEnum> : ICustomOption<TEnum>, ICustomStringOption
    where TEnum : Enum
{
}

public interface ICustomStringOption : ICustomOption
{
    public int ValueInt { get; set; }
    
    public StringNames[] ValueNames { get; }
}