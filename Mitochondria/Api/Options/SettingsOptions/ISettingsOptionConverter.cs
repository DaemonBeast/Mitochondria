using Mitochondria.Api.Binding;

namespace Mitochondria.Api.Options.SettingsOptions;

public interface ISettingsOptionConverter<TConvertedType>
    : ISettingsOptionConverter, IConverter<TConvertedType, TransformConvertArgs>
    where TConvertedType : OptionBehaviour
{
}

public interface ISettingsOptionConverter
{
}