using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Framework.Utilities;
using Mitochondria.Core.Framework.Utilities.DataStructures;

namespace Mitochondria.Core.Framework.Binding;

public class ConverterManager
{
    public static ConverterManager Instance => Singleton<ConverterManager>.Instance;

    private TypeGraph<IConverter> Converters => TypeGraph<IConverter>.Shared;

    private readonly MethodInfo _registerMethodInfo;

    private ConverterManager()
    {
        _registerMethodInfo = typeof(ConverterManager).GetMethod(nameof(Register), Type.EmptyTypes)!;
    }

    public IConverter Register(Type converterType)
    {
        if (!typeof(IConverter).IsAssignableFrom(converterType))
        {
            throw new ArgumentException($"Converter must inherit {typeof(IConverter).FullName}");
        }

        return (IConverter) _registerMethodInfo.MakeGenericMethod(converterType).Invoke(this, Array.Empty<object?>())!;
    }

    public TConverter Register<TConverter>()
        where TConverter : class, IConverter
    {
        var converter = Singleton<TConverter>.Instance;
        Converters.Add(converter.ConvertedType, converter);

        return converter;
    }

    public IEnumerable<IConverter> ToType<TConvertedType>()
        => ToType(typeof(TConvertedType));

    public IEnumerable<IConverter> ToType(Type convertedType)
        => Converters.Traverse(convertedType);

    public bool TryConvert<TConvertedType, TConvertArgs>(
        object objToConvert,
        [NotNullWhen(true)] out TConvertedType? convertedObj,
        TConvertArgs convertArgs)
        where TConvertedType : class
    {
        foreach (var converter in ToType<TConvertedType>())
        {
            if (converter.ConvertsTo(typeof(TConvertedType)) &&
                converter is IConverter<TConvertArgs> typedConverter &&
                typedConverter.UnsafeTryConvert(objToConvert, out convertedObj, convertArgs))
            {
                return true;
            }
        }

        convertedObj = default;
        return false;
    }
}