using Mitochondria.Core.Api.GUI;
using Mitochondria.Core.Framework.Helpers;

namespace Mitochondria.Options.Settings.Framework.Gui;

public class SettingsOptionContainer : GuiContainer<SettingsOptionElement>, IOrderableContainer
{
    public float Gap { get; set; }

    public IEnumerable<SettingsOptionElement> ActiveOrderedChildren =>
        ChildrenWrapper.Actual.Where(g => g.GameObject!.activeInHierarchy);

    private const float OptionOffset = 2.1f;
    private const float DefaultOptionOffset = 0.5f;

    private const float ScrollOffset = -2.15f;

    private SettingsOptionContainer()
    {
        Gap = DefaultOptionOffset;
    }

    protected override void OnAdd(SettingsOptionElement guiElement)
    {
        Order();
        TryUpdateLayout();
    }

    protected override void OnRemove(SettingsOptionElement guiElement)
        => TryUpdateLayout();

    public void Order()
    {
        ChildrenWrapper.Actual = ChildrenWrapper.Actual.OrderBy(s => s.Order).ToList();
        ChildrenWrapper.MarkDirty();
    }

    private static void TryMoveSettingsOptionTo(IGuiElement guiElement, float y)
    {
        var transform = guiElement.GameObject!.transform;
        var localPosition = transform.localPosition;

        localPosition.y = y;
        transform.localPosition = localPosition;
    }
    
    private void TryMoveSettingsOptions()
    {
        var currentOffset = OptionOffset;

        foreach (var settingsOptionFlex in Children.Where(e => e.GameObject!.activeInHierarchy))
        {
            TryMoveSettingsOptionTo(settingsOptionFlex, currentOffset);

            currentOffset -= Gap;
        }
    }

    private void TryChangeMenuHeight()
    {
        if (!GameOptionsMenuHelper.TryGetCurrent(out var gameOptionsMenu))
        {
            return;
        }

        var scroller = gameOptionsMenu.GetComponentInParent<Scroller>();
        if (scroller == null)
        {
            return;
        }
        
        scroller.ContentYBounds.max = ScrollOffset - OptionOffset + ChildrenWrapper.Length * Gap;
    }

    private bool TryUpdateLayout()
    {
        if (!GameOptionsMenuHelper.TryGetCurrent(out _))
        {
            return true;
        }
        
        TryMoveSettingsOptions();
        TryChangeMenuHeight();

        return true;
    }
}