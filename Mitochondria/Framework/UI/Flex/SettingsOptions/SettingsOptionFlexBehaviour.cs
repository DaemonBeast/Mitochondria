using Il2CppInterop.Runtime.Attributes;
using Mitochondria.Framework.Options.SettingsOptions.Managers;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Mitochondria.Framework.UI.Flex.SettingsOptions;

[RegisterInIl2Cpp]
public class SettingsOptionFlexBehaviour : MonoBehaviour
{
    [HideFromIl2Cpp]
    public SettingsOptionFlex? Flex { get; internal set; }
    
    private void OnDestroy()
    {
        var flexContainer = SettingsOptionManager.Instance.FlexContainer;
        if (flexContainer.CanRemove && Flex != null)
        {
            flexContainer.TryRemove(Flex);
        }
    }
}