using Mitochondria.Api.Binding;
using Mitochondria.Api.UI.Hud;
using Mitochondria.Api.UI.Hud.Buttons;
using Mitochondria.Framework.Binding;
using Mitochondria.Framework.Helpers;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;

namespace Mitochondria.Framework.UI.Hud.Containers;

public class MainActionButtonsContainer : HudContainer<ICustomActionButton>
{
    private readonly Dictionary<ICustomActionButton, ActionButton> _actionButtons;

    public MainActionButtonsContainer()
    {
        _actionButtons = new Dictionary<ICustomActionButton, ActionButton>();
    }

    protected override void OnRemove(ICustomActionButton hudElement)
    {
        if (_actionButtons.TryGetValue(hudElement, out var actionButton) && _actionButtons.Remove(hudElement))
        {
            actionButton.Destroy();
        }
    }

    internal void CreateActionButtons()
    {
        if (HudManager.InstanceExists)
        {
            var convertArgs = new TransformConvertArgs(HudManagerHelper.Transforms.BottomRightButtons);

            foreach (var customActionButton in Children)
            {
                if (_actionButtons.TryGetValue(customActionButton, out var actionButton))
                {
                    if (actionButton != null)
                    {
                        continue;
                    }

                    _actionButtons.Remove(customActionButton);
                }

                if (!Converter.Instance.TryConvert(
                        customActionButton,
                        out actionButton,
                        convertArgs))
                {
                    Logger<MitochondriaPlugin>.Error("Failed to convert to action button");
                    continue;
                }

                if (!Binder.Instance.TryBind(actionButton, customActionButton))
                {
                    actionButton.Destroy();
                    Logger<MitochondriaPlugin>.Error("Failed to bind to action button");
                    continue;
                }

                _actionButtons.Add(customActionButton, actionButton);
            }
        }
    }
}