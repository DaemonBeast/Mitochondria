using Mitochondria.Api.Binding;
using Mitochondria.Framework.Binding;
using Mitochondria.Framework.Options.SettingsOptions.Converters;

namespace Mitochondria.Framework.Options.SettingsOptions.Bindings;

[Binding]
public class SettingsStringOptionBinding : Binding<StringOption, ICustomStringOption>
{
    public override void BindEvents()
    {
        SettingsOptionBindingHelper.Bind(
            Obj,
            Other,
            () => Other.ValueInt = Obj.Value,
            () => Obj.Value = Other.ValueInt);
    }
}