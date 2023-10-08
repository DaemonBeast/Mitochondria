using BepInEx;
using Mitochondria.Api.Services;
using Mitochondria.Api.UI.Flex;
using Mitochondria.Framework.Helpers;
using Mitochondria.Framework.Services;
using Mitochondria.Framework.UI.Flex;
using Mitochondria.Framework.UI.Flex.SettingsOptions;
using UnityEngine;

namespace Mitochondria.Patches.Options;

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