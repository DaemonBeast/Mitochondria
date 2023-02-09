using BepInEx.Unity.IL2CPP;
using Mitochondria.Framework.Utilities.Extensions;
using Reactor.Localization.Utilities;

namespace Mitochondria.Api.Options;

public abstract class CustomOption<TPlugin, TValue> : ICustomOption<TValue>
    where TPlugin : BasePlugin
    where TValue : notnull
{
    public StringNames TitleName { get; }

    public string Title { get; }
    
    public virtual string ValueString => string.Format(FormatString, Value);

    public Type ValueType { get; }

    public string FormatString { get; }

    public TValue Value
    {
        get => _value;
        set => SetAndInvokeEvent(value);
    }
    
    public TValue DefaultValue { get; }

    public event ICustomOption<TValue>.ValueChangedHandler? OnValueChanged;

    public event ICustomOption.ChangedHandler? OnChanged;

    private TValue _value;

    private const string DefaultFormatString = "{0}";

    public CustomOption(
        string title,
        TValue value,
        string? formatString = null)
    {
        Title = title;
        ValueType = typeof(TValue);
        _value = value;
        DefaultValue = value;
        FormatString = formatString ?? DefaultFormatString;

        TitleName = CustomStringName.CreateAndRegister(Title);

        this.SetOwner<TPlugin>();
    }

    public void ResetValue()
    {
        Value = DefaultValue;
    }

    public bool HasDefaultValue()
        => EqualityComparer<TValue>.Default.Equals(Value, DefaultValue);

    private void SetAndInvokeEvent(TValue newValue)
    {
        if (EqualityComparer<TValue>.Default.Equals(Value, newValue))
        {
            return;
        }

        var oldValue = _value;
        _value = newValue;

        OnValueChanged?.Invoke(this, oldValue, newValue);
        OnChanged?.Invoke(this);
    }
}