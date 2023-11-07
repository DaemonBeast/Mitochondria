using Il2CppInterop.Runtime.Attributes;
using Mitochondria.Core.Api.GUI;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Mitochondria.Core.Framework.GUI;

[RegisterInIl2Cpp]
internal class GuiElementBehaviour : MonoBehaviour
{
    [HideFromIl2Cpp]
    public IGuiElement? Element { get; set; }

    private void OnDestroy()
    {
        Element?.Container?.Remove(Element);
    }
}