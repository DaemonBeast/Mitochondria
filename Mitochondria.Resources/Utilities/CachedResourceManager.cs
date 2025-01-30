using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Mitochondria.Resources.Utilities;

public static class CachedResourceManager
{
    private static readonly Dictionary<Type, MethodInfo> Cache = new();

    private static readonly MethodInfo CreateCompletedOperationMethodInfo = typeof(ResourceManager)
        .GetMethod(nameof(ResourceManager.CreateCompletedOperation), AccessTools.all)!;

    public static AsyncOperationHandle CreateCompletedOperationHandle(
        Type resultType,
        object result,
        string errorMsg = "")
    {
        if (!Cache.TryGetValue(resultType, out var createCompletedOperation))
        {
            createCompletedOperation = CreateCompletedOperationMethodInfo.MakeGenericMethod(resultType);
            Cache.Add(resultType, createCompletedOperation);
        }

        var boxedCompletedOperation = (Il2CppObjectBase) createCompletedOperation.Invoke(
            UnityEngine.AddressableAssets.Addressables.ResourceManager, new[]
            {
                result,
                errorMsg
            })!;

        // I'm not sure if this is safe, but it seems to work.
        return new AsyncOperationHandle(boxedCompletedOperation.Pointer);
    }
}
