using Mitochondria.Core.Api.Binding;

namespace Mitochondria.Core.Api.Options.SettingsOptions;

public interface ISettingsOptionConverter<TConvertedType>
    : ISettingsOptionConverter, IConverter<TConvertedType, TransformConvertArgs>
    where TConvertedType : OptionBehaviour
{
}

public interface ISettingsOptionConverter
{
}