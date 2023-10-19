using Mitochondria.Api.Services;
using Mitochondria.Framework.Modifiers;
using Mitochondria.Framework.Services;

namespace Mitochondria.Patches.Modifiers;

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