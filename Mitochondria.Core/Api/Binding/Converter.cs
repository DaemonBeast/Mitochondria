using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Mitochondria.Core.Api.Binding;

public abstract class Converter<TConvertedType, TConvertArgs> : IConverter<TConvertArgs>
    where TConvertedType : class
{
    public Type ConvertedType => typeof(TConvertedType);

    public virtual bool ConvertsTo(Type convertedType)
        => convertedType.IsAssignableFrom(typeof(TConvertedType));

    public abstract bool TryConvert(
        object objToConvert,
        [NotNullWhen(true)] out TConvertedType? convertedObj,
        TConvertArgs convertArgs);

    public bool UnsafeTryConvert<TUnsafeConvertedType>(
        object objToConvert,
        [NotNullWhen(true)] out TUnsafeConvertedType? unsafeConvertedObj,
        TConvertArgs convertArgs)
        where TUnsafeConvertedType : class
    {
        var result = TryConvert(objToConvert, out var convertedObj, convertArgs);

        unsafeConvertedObj = convertedObj as TUnsafeConvertedType;
        return result;
    }
}

public interface IConverter<in TConvertArgs> : IConverter
{
    public bool UnsafeTryConvert<TUnsafeConvertedType>(
        object objToConvert,
        [NotNullWhen(true)] out TUnsafeConvertedType? convertedObj,
        TConvertArgs convertArgs)
        where TUnsafeConvertedType : class;
}

public interface IConverter
{
    public Type ConvertedType { get; }

    public bool ConvertsTo(Type convertedType);
}

public readonly record struct EmptyConvertArgs;

public readonly record struct TransformConvertArgs(Transform? Parent = null);