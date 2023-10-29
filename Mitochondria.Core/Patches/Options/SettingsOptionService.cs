using BepInEx;
using Mitochondria.Core.Api.Services;
using Mitochondria.Core.Api.UI.Flex;
using Mitochondria.Core.Framework.Helpers;
using Mitochondria.Core.Framework.Services;
using Mitochondria.Core.Framework.UI.Flex;
using Mitochondria.Core.Framework.UI.Flex.SettingsOptions;
using UnityEngine;

namespace Mitochondria.Core.Patches.Options;

[Service]
public class SettingsOptionService : IService
{
    private int _orderCounter;

    private SettingsOptionService()
    {
        _orderCounter = 0;
    }

    public void OnPluginLoaded(PluginInfo pluginInfo)
    {
        FlexGameObjectManager.Instance.DefineFlex<OptionBehaviour>(
            CreateSettingsOptionFlex,
            Constants.Scenes.InGame,
            "Game Settings");
        
        FlexGameObjectManager.Instance.DefineFlex<OptionBehaviour>(
            CreateSettingsOptionFlex,
            Constants.Scenes.InGame,
            "HideNSeekOptionsMenu");

        GameOptionsMenuHelper.OnBeforeOpened += () => _orderCounter = 0;
    }

    private IFlex CreateSettingsOptionFlex(GameObject gameObject)
    {
        var flex = new SettingsOptionFlex(gameObject, _orderCounter += 10);
        gameObject.AddComponent<SettingsOptionFlexBehaviour>().Flex = flex;

        return flex;
    }
}