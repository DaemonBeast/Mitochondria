using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mitochondria.Core.Api.Binding;
using Mitochondria.Core.Framework.Utilities;

namespace Mitochondria.Core.Framework.Binding;

public class Converter<TConverter>
    where TConverter : class, IConverter
{
    public TConverter Instance =>
        _instance ??= Converter.Instance.Get<TConverter>() ?? Converter.Instance.Register<TConverter>();

    private TConverter? _instance;
}

public class Converter
{
    public static Converter Instance => Singleton<Converter>.Instance;

    private static TypeGraph<IConverter> Converters => TypeGraph<IConverter>.Shared;

    private readonly Dictionary<Type, IConverter> _converterMap;
    private readonly MethodInfo _registerMethodInfo;

    private Converter()
    {
        _converterMap = new Dictionary<Type, IConverter>();
        _registerMethodInfo = typeof(Converter).GetMethod(nameof(Register), Type.EmptyTypes)!;
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
        _converterMap[typeof(TConverter)] = converter;

        return converter;
    }

    public IConverter[] ToType<T>()
        => ToType(typeof(T));

    public IConverter[] ToType(Type convertedType)
        => Converters.TryGet(convertedType, out var typeGraph)
            ? typeGraph.Traverse().ToArray()
            : Array.Empty<IConverter>();

    public TConverter? Get<TConverter>()
        where TConverter : class, IConverter
        => Get(typeof(TConverter)) as TConverter;

    public IConverter? Get(Type converterType)
        => _converterMap.TryGetValue(converterType, out var converter) ? converter : null;

    public bool TryConvert<TConvertedType, TConvertArgs>(
        object objToConvert,
        [NotNullWhen(true)] out TConvertedType? convertedObj,
        TConvertArgs convertArgs) where TConvertedType : class
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