using HarmonyLib;
using InnerNet;
using Mitochondria.Core.Framework.Modifiers;

namespace Mitochondria.Core.Patches.Modifiers;

public static class GameplayModifierPatches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
    public static class OnPlayerJoinedHookPatch
    {
        public static void Postfix(ClientData data)
        {
            foreach (var modifier in ModifierManager.Instance.Get<GameplayModifier>())
            {
                modifier.OnPlayerJoined(data);
            }
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerLeft))]
    public static class OnPlayerLeftHookPatch
    {
        public static void Postfix(ClientData data, DisconnectReasons reason)
        {
            foreach (var modifier in ModifierManager.Instance.Get<GameplayModifier>())
            {
                modifier.OnPlayerLeft(data, reason);
            }
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnBecomeHost))]
    public static class OnBecomeHostHookPatch
    {
        public static void Postfix()
        {
            foreach (var modifier in ModifierManager.Instance.Get<GameplayModifier>())
            {
                modifier.OnBecomeHost();
            }
        }
    }

    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__33), nameof(IntroCutscene._CoBegin_d__33.MoveNext))]
    public static class IntroCutsceneStartHookPatch
    {
        public static void Postfix(IntroCutscene._CoBegin_d__33 __instance)
        {
            if (__instance.__1__state == 1)
            {
                var introCutscene = __instance.__4__this;

                foreach (var modifier in ModifierManager.Instance.Get<GameplayModifier>())
                {
                    modifier.OnIntroCutsceneStarted(introCutscene);
                }
            }
        }
    }

    [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__36), nameof(IntroCutscene._ShowTeam_d__36.MoveNext))]
    public static class IntroCutsceneTeamHookPatch
    {
        public static void Prefix(IntroCutscene._ShowTeam_d__36 __instance, out bool __state)
        {
            __state = __instance.__1__state == 1;
        }

        public static void Postfix(IntroCutscene._ShowTeam_d__36 __instance, bool __state)
        {
            if (__state)
            {
                var introCutscene = __instance.__4__this;

                foreach (var modifier in ModifierManager.Instance.Get<GameplayModifier>())
                {
                    modifier.OnTeamRevealed(introCutscene);
                }
            }
        }
    }

    [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__39), nameof(IntroCutscene._ShowRole_d__39.MoveNext))]
    public static class IntroCutsceneRoleHookPatch
    {
        public static void Postfix(IntroCutscene._ShowRole_d__39 __instance)
        {
            if (__instance.__1__state == 1)
            {
                var introCutscene = __instance.__4__this;

                foreach (var modifier in ModifierManager.Instance.Get<GameplayModifier>())
                {
                    modifier.OnRoleRevealed(introCutscene);
                }
            }
        }
    }

    [HarmonyPatch(typeof(HudManager._CoShowIntro_d__87), nameof(HudManager._CoShowIntro_d__87.MoveNext))]
    public static class IntroCutsceneEndingHookPatch
    {
        public static void Prefix(HudManager._CoShowIntro_d__87 __instance)
        {
            if (__instance.__1__state == 4)
            {
                foreach (var modifier in ModifierManager.Instance.Get<GameplayModifier>())
                {
                    modifier.OnIntroCutsceneEnding();
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.StartGame))]
    public static class GameStartedHookPatch
    {
        public static void Postfix(GameManager __instance)
        {
            foreach (var modifier in ModifierManager.Instance.Get<GameplayModifier>())
            {
                modifier.OnGameStarted(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.EndGame))]
    public static class GameEndedHookPatch
    {
        public static void Postfix(GameManager __instance)
        {
            foreach (var modifier in ModifierManager.Instance.Get<GameplayModifier>())
            {
                modifier.OnGameEnded(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.DisconnectInternal))]
    public static class LeftGameHookPatch
    {
        public static void Postfix()
        {
            foreach (var modifier in ModifierManager.Instance.Get<GameplayModifier>())
            {
                modifier.OnDisconnect();
            }
        }
    }
}