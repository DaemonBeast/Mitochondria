using Mitochondria.Core.Api.Services;
using Mitochondria.Core.Framework.Modifiers;
using Mitochondria.Core.Framework.Services;

namespace Mitochondria.Core.Patches.Modifiers;

[Service]
public class ModifierService : IService
{
    public void OnUpdate()
    {
        foreach (var modifier in ModifierManager.Instance.AllModifiers)
        {
            modifier.OnUpdate();
        }
    }
}