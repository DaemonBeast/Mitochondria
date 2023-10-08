using Mitochondria.Api.Binding;
using Mitochondria.Framework.Binding;
using Mitochondria.Framework.Options.SettingsOptions.Converters;

namespace Mitochondria.Framework.Options.SettingsOptions.Bindings;

[Binding]
public class SettingsNumberOptionBinding : Binding<NumberOption, ICustomNumberOption>
{
    public override void BindEvents()
    {
        SettingsOptionBindingHelper.Bind(
            Obj,
            Other,
            () => Other.Value = Obj.Value,
            () => Obj.Value = Other.Value);
    }
}