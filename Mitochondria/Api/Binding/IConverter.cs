using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Mitochondria.Api.Binding;

public interface IConverter<TConvertedType, TConvertArgs> : IConverter<TConvertArgs>
where TConvertedType : class
{
    public bool TryConvert(
        object objToConvert,
        [NotNullWhen(true)] out TConvertedType? convertedObj,
        TConvertArgs convertArgs);

    bool IConverter<TConvertArgs>.UnsafeTryConvert<TUnsafeConvertedType>(
        object objToConvert,
        [NotNullWhen(true)] out TUnsafeConvertedType? unsafeConvertedObj,
        TConvertArgs convertArgs) where TUnsafeConvertedType : class
    {
        var result = TryConvert(objToConvert, out var convertedObj, convertArgs);
        
        unsafeConvertedObj = convertedObj as TUnsafeConvertedType;
        return result;
    }
}

public interface IConverter<TConvertArgs> : IConverter
{
    public bool UnsafeTryConvert<TUnsafeConvertedType>(
        object objToConvert,
        [NotNullWhen(true)] out TUnsafeConvertedType? convertedObj,
        TConvertArgs convertArgs) where TUnsafeConvertedType : class;
}

public interface IConverter
{
    public Type ConvertedType { get; }

    public bool ConvertsTo(Type convertedType);
}

public record struct EmptyConvertArgs;

public record struct TransformConvertArgs(Transform? Parent = null);