using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Framework.Binding;

namespace Mitochondria.Core.Framework.Options.SettingsOptions.Bindings;

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