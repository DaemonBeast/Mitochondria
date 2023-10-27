using Mitochondria.Api.Binding;
using Mitochondria.Api.UI.Hud.Buttons;
using Mitochondria.Framework.Binding;
using Mitochondria.Framework.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Mitochondria.Framework.UI.Hud.Buttons.Bindings;

[Binding]
public class ActionButtonBinding : Binding<ActionButton, CustomActionButton>
{
    private static readonly int Percent = Shader.PropertyToID("_Percent");

    public TextStyle TitleStyle { get; private set; }

    public bool Visible { get; private set; }

    public bool Enabled { get; private set; }

    public FillState FillState { get; private set; }

    public bool IsUnlimitedUses { get; private set; }

    public int? UsesRemaining { get; private set; }

    private ActionButtonBinding()
    {
        TitleStyle = TextStyle.Normal;
        Visible = true;
        FillState = FillState.Idle;
        IsUnlimitedUses = false;
        UsesRemaining = null;
    }

    public override void Init()
    {
        Enabled = Obj.canInteract;
    }

    public override void BindEvents()
    {
        var button = Obj.GetComponent<PassiveButton>();
        button.TargetActionButton = Obj;
        button.OnClick = new Button.ButtonClickedEvent();
        button.OnClick.AddListener((Action) (() =>
        {
            if (Enabled)
            {
                Other.InvokeClick();
            }
        }));
    }

    public override void Update()
    {
        TitleStyle.Equalize(
            Other.TitleStyle,
            titleStyle =>
            {
                TitleStyle = titleStyle;

                Obj.buttonLabelText.fontMaterial = titleStyle.Material;
            });

        Visible.Equalize(
            Other.Visible && Other.CouldUse(),
            visible =>
            {
                Visible = visible;

                if (Visible)
                {
                    Obj.Show();
                }
                else
                {
                    Obj.Hide();
                }
            });

        Enabled.Equalize(
            Obj.canInteract,
            Other is { Enabled: true, State.Uses.HasUsesRemaining: true } && Other.CanUse(),
            enabled => Enabled = Other.Enabled = enabled,
            enabled =>
            {
                Enabled = enabled;

                if (Enabled)
                {
                    Obj.SetEnabled();
                }
                else
                {
                    Obj.SetDisabled();
                }
            });

        FillState.Equalize(
            Other.State.FillState,
            fillState =>
            {
                FillState = fillState;
                Obj.graphic.transform.localPosition = Obj.position;

                if (FillState == FillState.Idle)
                {
                    var cooldownTime = Other.State.CooldownTime;
                    var fill = cooldownTime == 0f ? 0f : Mathf.Clamp(Other.State.FillAmount / cooldownTime, 0f, 1f);
                    Obj.SetCooldownFill(fill);
                    Obj.cooldownTimerText.gameObject.SetActive(false);
                }
            });

        switch (Other.State.FillState)
        {
            case FillState.Filling:
            {
                var maxUseTime = Other.State.MaxUseTime!.Value;
                Obj.SetFillUp(maxUseTime - Other.State.FillAmount, maxUseTime);

                break;
            }
            case FillState.Cooling:
            {
                Obj.SetCoolDown(Other.State.FillAmount, Other.State.CooldownTime);
                break;
            }
        }

        IsUnlimitedUses.Equalize(
            Other.State.Uses.IsUnlimited,
            isUnlimited =>
            {
                IsUnlimitedUses = isUnlimited;

                if (IsUnlimitedUses)
                {
                    Obj.SetInfiniteUses();
                }
            });

        UsesRemaining.Equalize(
            Other.State.Uses.Remaining,
            remaining =>
            {
                UsesRemaining = remaining;

                if (UsesRemaining != null)
                {
                    Obj.SetUsesRemaining(UsesRemaining.Value);
                }
            });
    }
}