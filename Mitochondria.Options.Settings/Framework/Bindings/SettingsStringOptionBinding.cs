using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Framework.Binding;
using Mitochondria.Core.Framework.Options;

namespace Mitochondria.Options.Settings.Framework.Bindings;

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