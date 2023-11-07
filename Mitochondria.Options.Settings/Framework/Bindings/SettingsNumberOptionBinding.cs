using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Framework.Binding;
using Mitochondria.Core.Framework.Options;

namespace Mitochondria.Options.Settings.Framework.Bindings;

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