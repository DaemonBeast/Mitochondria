using Mitochondria.Core.Api.Services;
using Mitochondria.Core.Framework.Binding;
using Mitochondria.Core.Framework.Services;

namespace Mitochondria.Core.Patches.Binding;

[Service]
public class BindingService : IService
{
    public void OnUpdate()
    {
        var bindings = Binder.Instance.Bindings;

        foreach (var binding in bindings)
        {
            if (binding.IsInvalid())
            {
                Binder.Instance.Remove(binding);
                continue;
            }

            binding.Update();
        }
    }
}