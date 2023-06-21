using BepInEx.Unity.IL2CPP;
using Hazel;
using Mitochondria.Framework.Networking;
using Mitochondria.Framework.Utilities.Extensions;
using Reactor.Localization.Utilities;
using Reactor.Networking.Serialization;

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

    public bool Sync { get; }

    public TValue Value
    {
        get => _value;
        set => SetAndInvokeEvent(value);
    }
    
    public TValue DefaultValue { get; }
    
    public string Id { get; }

    public bool HostOnly { get; }

    public event ICustomOption<TValue>.ValueChangedHandler? OnValueChanged;

    public event ICustomOption.ChangedHandler? OnChanged;

    private TValue _value;

    private const string DefaultFormatString = "{0}";

    protected CustomOption(
        string title,
        TValue value,
        string? formatString = null,
        bool sync = true)
    {
        Title = title;
        ValueType = typeof(TValue);
        _value = value;
        DefaultValue = value;
        FormatString = formatString ?? DefaultFormatString;
        Sync = sync;
        HostOnly = sync;

        TitleName = CustomStringName.CreateAndRegister(Title);
        Id = $"CustomOption_{Title}";

        this.SetOwner<TPlugin>();

        if (sync)
        {
            SyncableManager.Instance.Add(this);
        }
    }

    public void ResetValue()
    {
        Value = DefaultValue;
    }

    public bool HasDefaultValue()
        => EqualityComparer<TValue>.Default.Equals(Value, DefaultValue);
    
    public void Serialize(MessageWriter writer)
    {
        writer.Serialize(Value);
    }

    public void Deserialize(MessageReader reader)
    {
        Value = (TValue) reader.Deserialize(ValueType);
    }

    public override string ToString()
    {
        return $"{Title}: {ValueString}";
    }

    private void SetAndInvokeEvent(TValue newValue)
    {
        if (EqualityComparer<TValue>.Default.Equals(Value, newValue))
        {
            return;
        }

        var oldValue = _value;
        _value = newValue;

        if (Sync)
        {
            SyncableManager.Instance.Sync(this);
        }

        OnValueChanged?.Invoke(this, oldValue, newValue);
        OnChanged?.Invoke(this);
    }
}