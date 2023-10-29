using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Framework.Binding;

namespace Mitochondria.Core.Framework.Options.SettingsOptions.Bindings;

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