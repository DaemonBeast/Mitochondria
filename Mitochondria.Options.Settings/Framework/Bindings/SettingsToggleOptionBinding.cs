using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Api.Options;
using Mitochondria.Core.Framework.Binding;

namespace Mitochondria.Options.Settings.Framework.Bindings;

[Binding]
public class SettingsToggleOptionBinding : Binding<ToggleOption, ICustomOption<bool>>
{
    public override void BindEvents()
    {
        SettingsOptionBindingHelper.Bind(
            Obj,
            Other,
            () => Other.Value = Obj.CheckMark.enabled,
            () => Obj.CheckMark.enabled = Other.Value);
    }
}