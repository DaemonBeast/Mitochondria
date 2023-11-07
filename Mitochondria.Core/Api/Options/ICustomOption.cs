using Mitochondria.Core.Api.Networking;
using Mitochondria.Core.Api.Plugin;

namespace Mitochondria.Core.Api.Options;

public interface ICustomOption<TValue> : ICustomOption
    where TValue : notnull
{
    public TValue Value { get; set; }
    
    public TValue DefaultValue { get; }

    public delegate void ValueChangedHandler(ICustomOption customOption, TValue oldValue, TValue newValue);

    public event ValueChangedHandler? OnValueChanged;
}

public interface ICustomOption : IOwned, ISyncable
{
    public StringNames TitleName { get; }
    
    public string Title { get; }
    
    public string ValueString { get; }
    
    public Type ValueType { get; }

    public object BoxedValue { get; set; }

    public object BoxedDefaultValue { get; }
    
    public string FormatString { get; }
    
    public bool Sync { get; }

    public delegate void ChangedHandler(ICustomOption customOption);

    public event ChangedHandler? OnChanged;

    public void ResetValue();

    public bool HasDefaultValue();
}