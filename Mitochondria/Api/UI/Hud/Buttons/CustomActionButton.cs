using System.Collections;
using Mitochondria.Framework.Resources.Sprites;
using Reactor.Utilities;
using UnityEngine;

namespace Mitochondria.Api.UI.Hud.Buttons;

public abstract class CustomActionButton : ICustomActionButton
{
    public string Title { get; }

    public SpriteProvider Icon { get; }

    public string? Description { get; }

    public bool Visible { get; set; }
    
    public bool Enabled { get; set; }

    public IActionButtonState State { get; }

    private IEnumerator? _currentAdvanceEnumerator;

    public CustomActionButton(
        string title,
        SpriteProvider icon,
        string? description = null,
        int cooldownTime = 0,
        int maxUseTime = 0,
        int? uses = null)
    {
        Title = title;
        Icon = icon;
        Description = description;

        Visible = true;
        Enabled = true;

        State = new ActionButtonState(cooldownTime, maxUseTime, uses);
    }

    private bool _active;

    public virtual bool CouldUse()
        => true;

    public virtual bool CanUse()
        => true;

    public virtual void OnClick()
    {
    }

    public virtual void OnTimeUsedUp()
    {
    }

    public virtual void OnCooledDown()
    {
    }

    public void InvokeClick()
    {
        if (_active)
        {
            _active = false;
        }
        else if (Enabled && State is { HasCooledDown: true, Uses.HasUsesRemaining: true })
        {
            OnClick();
            _active = true;

            if (_currentAdvanceEnumerator != null)
            {
                Coroutines.Stop(_currentAdvanceEnumerator);
            }

            _currentAdvanceEnumerator = CoAdvanceState();
            Coroutines.Start(_currentAdvanceEnumerator);
        }
    }

    internal void AdvanceCooldown()
    {
        if (!_active && !State.HasCooledDown)
        {
            ((ActionButtonState) State).FillState = FillState.Cooling;

            State.FillAmount -= Time.deltaTime;

            if (State.HasCooledDown)
            {
                State.FillAmount = 0;
                ((ActionButtonState) State).FillState = FillState.Idle;

                OnCooledDown();
            }
        }
    }

    private IEnumerator CoAdvanceState()
    {
        State.FillAmount = 0;
        ((ActionButtonState) State).FillState = FillState.Filling;

        while (_active && !State.HasUsedUpTime)
        {
            yield return new WaitForFixedUpdate();
            State.FillAmount += Time.deltaTime;
        }

        _active = false;
        State.Uses.Decrement();
        State.FillAmount = State.CooldownTime;
        ((ActionButtonState) State).FillState = FillState.Idle;

        _currentAdvanceEnumerator = null;
        OnTimeUsedUp();
    }
}

public class ActionButtonState : IActionButtonState
{
    public float FillAmount { get; set; }

    public bool HasCooledDown => FillAmount <= 0f;

    public bool HasUsedUpTime => MaxUseTime.HasValue && FillAmount >= MaxUseTime.Value;

    public int CooldownTime { get; set; }

    public FillState FillState { get; internal set; }

    public int? MaxUseTime { get; set; }

    public ICustomActionButtonUses Uses { get; }

    public ActionButtonState(int cooldownTime = 0, int maxUseTime = 0, int? uses = null)
    {
        CooldownTime = cooldownTime;
        MaxUseTime = maxUseTime;

        Uses = new CustomActionButtonUses(uses);
    }
}

public class CustomActionButtonUses : ICustomActionButtonUses
{
    public bool HasUsesRemaining => IsUnlimited || Remaining > 0;

    public bool IsUnlimited => Remaining == null;

    public int? Remaining { get; set; }

    public CustomActionButtonUses(int? uses = null)
    {
        Remaining = uses;
    }

    public void SetUnlimited()
        => Remaining = null;

    public void Decrement()
    {
        if (!IsUnlimited)
        {
            Remaining--;
        }
    }
}