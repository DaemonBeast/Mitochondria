using Mitochondria.Api.UI.Flex;
using Mitochondria.Framework.Cache;

namespace Mitochondria.Framework.UI.Flex.SettingsOptions;

public class SettingsOptionFlexContainer : FlexContainer<SettingsOptionFlex>
{
    public override bool CanAdd => true;

    public override bool CanRemove => true;

    public override bool CanBeOrdered => true;

    public override bool CanChangeDirection => false;

    public override bool CanChangeGap => false;

    public override bool CanFlexAlign => false;

    public override bool CanCrossAlign => false;

    private const float OptionOffset = 2.1f;
    private const float DefaultOptionOffset = 0.5f;
    
    private const float ScrollOffset = -2.15f;

    public SettingsOptionFlexContainer(float gap = DefaultOptionOffset) : base(gap: gap)
    {
    }

    protected override bool OnAdd(SettingsOptionFlex child)
        => TryUpdateLayout();

    protected override bool OnAddRange(IEnumerable<SettingsOptionFlex> children)
        => TryUpdateLayout();

    protected override bool OnRemove(SettingsOptionFlex child)
        => TryUpdateLayout();

    protected override bool OnSetGap(float gap)
        => TryUpdateLayout();

    private static void TryMoveSettingsOptionTo(SettingsOptionFlex settingsOptionFlex, float y)
    {
        if (settingsOptionFlex.GameObject == null)
        {
            return;
        }
        
        var transform = settingsOptionFlex.GameObject.transform;
        var localPosition = transform.localPosition;
        
        localPosition.y = y;
        transform.localPosition = localPosition;
    }
    
    private void TryMoveSettingsOptions()
    {
        var currentOffset = OptionOffset;

        foreach (var settingsOptionFlex in ActiveOrderedChildren)
        {
            TryMoveSettingsOptionTo(settingsOptionFlex, currentOffset);

            currentOffset -= Gap;
        }
    }

    private void TryChangeMenuHeight()
    {
        if (!MonoBehaviourProvider.TryGet(out GameOptionsMenu? gameOptionsMenu))
        {
            return;
        }

        var scroller = gameOptionsMenu.GetComponentInParent<Scroller>();
        if (scroller == null)
        {
            return;
        }
        
        scroller.ContentYBounds.max = ScrollOffset - OptionOffset + Length * Gap;
    }

    private bool TryUpdateLayout()
    {
        if (!MonoBehaviourProvider.TryGet(out GameOptionsMenu? _))
        {
            return true;
        }
        
        TryMoveSettingsOptions();
        TryChangeMenuHeight();

        return true;
    }
}