using Mitochondria.Api.Services;
using Mitochondria.Framework.Binding;
using Mitochondria.Framework.Services;

namespace Mitochondria.Patches.Binding;

[Service]
public class BindingService : IService
{
    void IService.OnUpdate()
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