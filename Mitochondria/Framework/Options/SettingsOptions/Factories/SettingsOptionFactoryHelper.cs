using Mitochondria.Framework.Cache;
using Reactor.Utilities;

namespace Mitochondria.Framework.Options.SettingsOptions.Factories;

public static class SettingsOptionFactoryHelper
{
    public static void AddHandler<TOptionBehaviour>(
        TOptionBehaviour optionBehaviour,
        Action<TOptionBehaviour>? valueChangedHandler)
        where TOptionBehaviour : OptionBehaviour
    {
        if (valueChangedHandler == null)
        {
            return;
        }

        if (!MonoBehaviourProvider.TryGet(out GameOptionsMenu? gameOptionsMenu))
        {
            Logger<MitochondriaPlugin>.Warning(
                "Failed to add handler to `OptionBehaviour` because the `GameOptionsMenu` could not be found");

            return;
        }
        
        optionBehaviour.OnValueChanged = (Action<OptionBehaviour>) (_ =>
        {
            valueChangedHandler.Invoke(optionBehaviour);
            gameOptionsMenu.ValueChanged(optionBehaviour);
        });
    }
}