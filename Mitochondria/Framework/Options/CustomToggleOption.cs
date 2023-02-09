using BepInEx.Unity.IL2CPP;
using Mitochondria.Api.Options;

namespace Mitochondria.Framework.Options;

public class CustomToggleOption<TPlugin> : CustomOption<TPlugin, bool>
    where TPlugin : BasePlugin
{
    public CustomToggleOption(string title, bool value) : base(title, value)
    {
    }
}