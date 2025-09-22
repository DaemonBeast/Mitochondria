using AmongUs.Data;
using AmongUs.GameOptions;
using HarmonyLib;
using Mitochondria.GameModes.Utilities.Extensions;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mitochondria.GameModes.Patches;

internal static class CreateGameOptionsPatches
{
    [HarmonyPatch(typeof(CreateGameOptions), nameof(CreateGameOptions.Start))]
    public static class ReplaceModeWithDropdownPatch
    {
        public static void Postfix(CreateGameOptions __instance)
        {
            var transform = __instance.transform.Find("ParentContent/Content/GeneralTab");

            var modeOption = transform.Find("ModeOptions")!;
            modeOption.gameObject.SetActive(false);

            var serverOption = transform.Find("ServerOption")!;
            var newModeOption = UnityEngine.Object.Instantiate(serverOption, serverOption.parent)!;
            newModeOption.name = "NewModeOption";
            newModeOption.position = modeOption.position;

            var newModeOptionBlackSquare = newModeOption.Find("BlackSquare")!;
            newModeOptionBlackSquare.localPosition -= new Vector3(0, 0.04f, 0);

            var newModeOptionBoxTexts = newModeOption.Find("ServerBox").GetComponentsInChildren<TextMeshPro>(true);

            var textTranslator = newModeOption.GetComponentInChildren<TextTranslatorTMP>();
            textTranslator.TargetText = StringNames.GameType;

            var serverDropdown = transform.Find("ServerDropdown")!;
            var newModeDropdown = UnityEngine.Object.Instantiate(serverDropdown, serverDropdown.parent);
            newModeDropdown.name = "NewModeDropdown";
            newModeDropdown.position += new Vector3(0, 1, 0);

            newModeDropdown.GetComponent<ServerDropdown>().Destroy();
            var modeDropdown = newModeDropdown.gameObject.AddComponent<ModeDropdown>();
            var modeDropdownPosition = modeDropdown.transform.localPosition;
            modeDropdownPosition.y = modeOption.transform.localPosition.y;
            modeDropdown.transform.localPosition = modeDropdownPosition;

            modeDropdown.OnModeSet = modeText =>
            {
                foreach (var text in newModeOptionBoxTexts)
                {
                    text.text = modeText;
                    text.ForceMeshUpdate();
                }
            };

            modeDropdown.OnModeSet.Invoke(
                TranslationController.Instance.GetString(
                    DataManager.Settings.Multiplayer.LastPlayedGameMode.GetStringName()));

            UpdateCapacityOption(__instance);

            var modePassiveButton = newModeOption.GetComponentInChildren<PassiveButton>();
            modePassiveButton.OnClick.RemoveAllListeners();
            modePassiveButton.OnClick = new Button.ButtonClickedEvent();
            modePassiveButton.OnClick.AddListener((Action) (() => newModeDropdown.gameObject.SetActive(true)));

            var modeBlockerPassiveButton = newModeDropdown.Find("ClickBlocker").GetComponent<PassiveButton>();
            modeBlockerPassiveButton.OnClick.RemoveAllListeners();
            modeBlockerPassiveButton.OnClick = new Button.ButtonClickedEvent();
            modeBlockerPassiveButton.OnClick.AddListener((Action) (() => modeDropdown.gameObject.SetActive(false)));

            var currentOption = newModeDropdown.Find("CurrentOption");

            var currentOptionText = currentOption.Find("Server Text");
            var currentOptionTextPosition = currentOptionText.transform.localPosition;
            currentOptionTextPosition.y = -0.1f;
            currentOptionText.transform.localPosition = currentOptionTextPosition;

            // ReSharper disable once StringLiteralTypo
            var currentOptionArrow = currentOption.Find("DrowdownArrow");
            var currentOptionArrowPosition = currentOptionArrow.transform.localPosition;
            currentOptionArrowPosition.y = -0.13f;
            currentOptionArrow.transform.localPosition = currentOptionArrowPosition;

            // TODO: Correct tooltip.
        }
    }

    [RegisterInIl2Cpp]
    public class ModeDropdown : MonoBehaviour
    {
        public Action<string>? OnModeSet;

        private SpriteRenderer? _background;
        private ObjectPoolBehavior? _buttonPool;
        private PassiveButton? _firstOption;
        private CreateGameOptions? _createGameOptions;

        private const float InitialYPos = -1.775f;
        private const float ButtonHeight = 0.55f;

        public ModeDropdown(IntPtr ptr) : base(ptr)
        {
        }

        private void Awake()
        {
            _background = transform.Find("Background").GetComponent<SpriteRenderer>();
            _buttonPool = GetComponent<ObjectPoolBehavior>();
            _firstOption = transform.Find("CurrentOption").GetComponent<PassiveButton>();
            _createGameOptions = GetComponentInParent<CreateGameOptions>();
        }

        private void OnEnable()
        {
            FillModeOptions();
            UpdateCapacityOption(_createGameOptions!);
        }

        private void FillModeOptions()
        {
            var currentGameMode = DataManager.Settings.Multiplayer.LastPlayedGameMode;

            _firstOption!.ChangeButtonText(
                TranslationController.Instance.GetString(currentGameMode.GetStringName()));

            var totalGameModesCount = GameModesHelpers.ModeToName.Count + CustomGameModeManager.GameModes.Count;

            var i = 0;

            foreach (var (gameMode, gameModeStringName) in GameModesHelpers.ModeToName)
            {
                if (gameMode == AmongUs.GameOptions.GameModes.None || gameMode == currentGameMode) continue;

                var serverListButton = _buttonPool!.Get<ServerListButton>();

                serverListButton.transform.localPosition = new Vector3(
                    0.0f,
                    InitialYPos - ButtonHeight * i,
                    -1f);

                serverListButton.transform.localScale = Vector3.one;
                serverListButton.Text.text = TranslationController.Instance.GetString(gameModeStringName);
                serverListButton.Text.ForceMeshUpdate();
                serverListButton.Button.OnClick.RemoveAllListeners();
                serverListButton.Button.OnClick.AddListener((Action) (() => ChooseOption(gameMode)));
                i++;
            }

            foreach (var (gameMode, customGameMode) in CustomGameModeManager.GameModes)
            {
                if (gameMode == AmongUs.GameOptions.GameModes.None || gameMode == currentGameMode) continue;

                var serverListButton = _buttonPool!.Get<ServerListButton>();

                serverListButton.transform.localPosition = new Vector3(
                    0.0f,
                    InitialYPos - ButtonHeight * i,
                    -1f);

                serverListButton.transform.localScale = Vector3.one;
                serverListButton.Text.text = TranslationController.Instance.GetString(customGameMode.Name);
                serverListButton.Text.ForceMeshUpdate();
                serverListButton.Button.OnClick.RemoveAllListeners();
                serverListButton.Button.OnClick.AddListener((Action) (() => ChooseOption(gameMode)));
                i++;
            }

            _background!.transform.localPosition =
                new Vector3(0.0f, 0.175f + 0.5f * -ButtonHeight * (totalGameModesCount - 1), 0.0f);

            _background.size = new Vector2(_background.size.x, ButtonHeight * (totalGameModesCount - 1) + 0.1f);
        }

        private void ChooseOption(AmongUs.GameOptions.GameModes gameMode)
        {
            OnModeSet?.Invoke(TranslationController.Instance.GetString(gameMode.GetStringName()));

            _createGameOptions!.SetGameMode(gameMode);

            UpdateCapacityOption(_createGameOptions);
            _createGameOptions.SwitchOptions(gameMode);

            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _buttonPool!.ReclaimAll();
        }
    }

    private static void UpdateCapacityOption(CreateGameOptions createGameOptions)
    {
        var capacityOption = createGameOptions.capacityOption;

        if (CustomGameModeManager.GameModes.TryGetValue(
                DataManager.Settings.Multiplayer.LastPlayedGameMode,
                out var customGameMode))
        {
            var playersConfiguration = customGameMode.Configuration.Players;
            var range = new FloatRange(playersConfiguration.MinPlayers, playersConfiguration.MaxPlayers);
            capacityOption.ValidRange = range;
        }
        else
        {
            var range = new FloatRange(4, 15);
            capacityOption.ValidRange = range;
        }

        capacityOption.Value = capacityOption.ValidRange.Clamp(capacityOption.Value);
        GameOptionsManager.Instance.GameHostOptions.SetInt(Int32OptionNames.MaxPlayers, (int) capacityOption.Value);

        createGameOptions.capacityOption.MinusBtn.SetInteractable(
            !Mathf.Approximately(capacityOption.Value, capacityOption.ValidRange.min));

        createGameOptions.capacityOption.PlusBtn.SetInteractable(
            !Mathf.Approximately(capacityOption.Value, capacityOption.ValidRange.max));
    }

    [HarmonyPatch(typeof(ConfirmCreatePopUp), nameof(ConfirmCreatePopUp.SetupInfo))]
    public static class GameModeTextPatch
    {
        public static void Postfix(ConfirmCreatePopUp __instance)
        {
            __instance.modeText.text =
                TranslationController.Instance.GetString(
                    DataManager.Settings.Multiplayer.LastPlayedGameMode.GetStringName());
        }
    }
}
