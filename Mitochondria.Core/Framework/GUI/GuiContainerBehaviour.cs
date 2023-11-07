using Il2CppInterop.Runtime.Attributes;
using Mitochondria.Core.Api.GUI;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Mitochondria.Core.Framework.GUI;

[RegisterInIl2Cpp]
internal class GuiContainerBehaviour : MonoBehaviour
{
    [HideFromIl2Cpp]
    public IGuiContainer? Container { get; set; }
}