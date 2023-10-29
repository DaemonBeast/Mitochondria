using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using Mitochondria.Core.Api.Roles;
using Mitochondria.Core.Framework.Options;
using Mitochondria.Core.Framework.Options.SettingsOptions;
using Mitochondria.Core.Framework.Options.SettingsOptions.Managers;

namespace Mitochondria.Core.Framework.Roles;

public static class RoleSideOptionHelper
{
    public static Func<IRoleSide, ICustomNumberOption> CreateFactory<TPlugin>(
        int defaultAmount = 0,
        int maxAmount = int.MaxValue)
        where TPlugin : BasePlugin
    {
        return roleSide => new CustomNumberOption<TPlugin>(
            $"# {roleSide.PluralTitle}",
            $"# {roleSide.PluralTitle}",
            defaultAmount,
            new FloatRange(0, maxAmount));
    }

    public static void TryAddSettingsOption<TPlugin>(
        IRoleSide roleSide,
        GameModes gameMode = GameModes.Normal,
        int order = 31)
        where TPlugin : BasePlugin
    {
        if (roleSide.AmountOption != null)
        {
            CustomSettingsOptionManager.Instance.Add(
                new CustomSettingsNumberOption<TPlugin>(roleSide.AmountOption, gameMode, order));
        }
    }
}