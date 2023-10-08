using Mitochondria.Api.Binding;
using Mitochondria.Api.Options;
using Mitochondria.Framework.Binding;
using Mitochondria.Framework.Options.SettingsOptions.Converters;

namespace Mitochondria.Framework.Options.SettingsOptions.Bindings;

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