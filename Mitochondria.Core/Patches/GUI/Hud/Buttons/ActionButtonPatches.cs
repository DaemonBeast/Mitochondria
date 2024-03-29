﻿using HarmonyLib;
using Mitochondria.Core.Framework.Prototypes;
using Mitochondria.Core.Framework.Resources.Cache;

namespace Mitochondria.Core.Patches.GUI.Hud.Buttons;

public static class ActionButtonPatches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class PrototypesPatch
    {
        [HarmonyPriority(Priority.VeryHigh)]
        public static void Prefix(HudManager __instance)
        {
            PrototypeManager.Instance.CloneAndSet(__instance.AbilityButton);
            PrototypeManager.Instance.CloneAndSet(__instance.AdminButton);
            PrototypeManager.Instance.CloneAndSet(__instance.ImpostorVentButton);
            PrototypeManager.Instance.CloneAndSet(__instance.KillButton);
            PrototypeManager.Instance.CloneAndSet(__instance.PetButton);
            PrototypeManager.Instance.CloneAndSet(__instance.ReportButton);
            PrototypeManager.Instance.CloneAndSet(__instance.SabotageButton);
            PrototypeManager.Instance.CloneAndSet(__instance.UseButton);

            MaterialCache.Instance.Set(
                MaterialCacheIds.RedAndWhiteText,
                __instance.KillButton.buttonLabelText.fontSharedMaterial);
        }
    }

    [HarmonyPatch(typeof(ActionButton), nameof(ActionButton.Start))]
    public static class CooldownShaderPatch
    {
        public static void Postfix(ActionButton __instance)
        {
            __instance.graphic.SetCooldownNormalizedUvs();
        }
    }
}