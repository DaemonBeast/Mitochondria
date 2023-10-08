using Il2CppInterop.Runtime.Attributes;
using Mitochondria.Api.Binding;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Mitochondria.Framework.Binding;

[RegisterInIl2Cpp]
public class BindingBehaviour : MonoBehaviour
{
    [HideFromIl2Cpp]
    public IBinding? Binding { get; internal set; }

    private void OnDestroy()
    {
        if (Binding != null)
        {
            Binder.Instance.Remove(Binding);
        }
    }
}