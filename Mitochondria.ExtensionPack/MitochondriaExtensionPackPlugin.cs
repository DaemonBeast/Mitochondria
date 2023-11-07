using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mitochondria.Core;
using Mitochondria.Core.Framework.Options;
using Mitochondria.Options.Settings.Framework;
using Reactor;

namespace Mitochondria.ExtensionPack;

// ReSharper disable once StringLiteralTypo
[BepInAutoPlugin("astral.mitochondria.extensionpack")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MitochondriaPlugin.Id)]
public partial class MitochondriaExtensionPackPlugin : BasePlugin
{
    public Harmony Harmony { get; }

    [CustomSettingsOption(order: 11)]
    [CustomSettingsOption(GameModes.HideNSeek, 11)]
    public static CustomToggleOption<MitochondriaPlugin> ShowTooltipsOption { get; private set; }

    static MitochondriaExtensionPackPlugin()
    {
        ShowTooltipsOption = new CustomToggleOption<MitochondriaPlugin>("Show Tooltips", "Show Tooltips", true, false);
    }

    public MitochondriaExtensionPackPlugin()
    {
        Harmony = new Harmony(Id);
    }

    public override void Load()
    {
        Harmony.PatchAll();
    }

    public override bool Unload()
    {
        Harmony.UnpatchSelf();

        return base.Unload();
    }
}