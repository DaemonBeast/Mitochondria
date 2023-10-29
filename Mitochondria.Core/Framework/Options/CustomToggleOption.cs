using BepInEx.Unity.IL2CPP;
using Mitochondria.Core.Api.Options;

namespace Mitochondria.Core.Framework.Options;

public class CustomToggleOption<TPlugin> : CustomOption<TPlugin, bool>
    where TPlugin : BasePlugin
{
    public CustomToggleOption(
        string id,
        string title,
        bool value,
        bool sync = true) : base(id, title, value, sync: sync)
    {
    }
}