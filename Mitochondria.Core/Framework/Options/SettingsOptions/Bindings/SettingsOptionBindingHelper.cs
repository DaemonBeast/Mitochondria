using Mitochondria.Core.Api.Options;

namespace Mitochondria.Core.Framework.Options.SettingsOptions.Bindings;

public static class SettingsOptionBindingHelper
{
    public static void Bind(
        OptionBehaviour optionBehaviour,
        ICustomOption customOption,
        Action optionBehaviourChangedHandler,
        Action customOptionChangedHandler)
    {
        optionBehaviour.OnValueChanged = (Action<OptionBehaviour>) (_ =>
        {
            optionBehaviourChangedHandler.Invoke();
            optionBehaviour.gameObject.GetComponentInParent<GameOptionsMenu>(true).ValueChanged(optionBehaviour);
        });

        void CustomOptionHandler(ICustomOption _)
        {
            if (optionBehaviour == null)
            {
                customOption.OnChanged -= CustomOptionHandler;
                return;
            }
            
            customOptionChangedHandler.Invoke();
            optionBehaviour.OnValueChanged.Invoke(optionBehaviour);
        }

        customOption.OnChanged += CustomOptionHandler;
    }
}