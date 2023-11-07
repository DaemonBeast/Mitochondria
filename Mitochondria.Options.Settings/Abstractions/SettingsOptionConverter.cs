using Mitochondria.Core.Api.Binding;

namespace Mitochondria.Options.Settings.Abstractions;

public abstract class SettingsOptionConverter<TConvertedType> : Core.Api.Binding.Converter<TConvertedType, TransformConvertArgs>
    where TConvertedType : OptionBehaviour
{
}