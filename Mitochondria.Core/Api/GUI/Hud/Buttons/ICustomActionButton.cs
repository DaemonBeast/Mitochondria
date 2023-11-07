using Mitochondria.Core.Framework.Resources.Sprites;

namespace Mitochondria.Core.Api.GUI.Hud.Buttons;

public interface ICustomActionButton : IHudElement
{
    public string Title { get; }

    public SpriteProvider Icon { get; }

    public string? Description { get; }

    public TextStyle TitleStyle { get; set; }

    public bool Visible { get; set; }

    public bool Enabled { get; set; }

    public IActionButtonState State { get; }

    public bool CouldUse();

    public bool CanUse();

    public void OnClick();

    public void OnTimeUsedUp();

    public void OnCooledDown();

    public void InvokeClick();
}

public interface IActionButtonState
{
    public float FillAmount { get; set; }

    public bool HasCooledDown { get; }

    public bool HasUsedUpTime { get; }

    public int CooldownTime { get; }

    public FillState FillState { get; }

    public int? MaxUseTime { get; }

    public ICustomActionButtonUses Uses { get; }
}

public interface ICustomActionButtonUses
{
    public bool HasUsesRemaining { get; }

    public bool IsUnlimited { get; }

    public int? Remaining { get; set; }

    public void SetUnlimited();

    public void Decrement();
}

public enum FillState
{
    Idle,
    Filling,
    Cooling
}