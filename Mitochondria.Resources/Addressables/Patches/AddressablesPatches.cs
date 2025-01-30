using System.Runtime.InteropServices;
using BepInEx.Unity.IL2CPP.Hook;
using HarmonyLib;
using Il2CppInterop.Common;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using Mitochondria.Resources.Utilities;
using Reactor.Utilities;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Mitochondria.Resources.Addressables.Patches;

internal static unsafe class AddressablesPatches
{
    private static readonly Dictionary<string, Handles> AllHandles = new();

    public static class LoadAssetPatch
    {
        // I'm not sure if the `methodInfoPtr` is actually a method info pointer; it just seems really funky.
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr LoadAssetAsyncDel(
            IntPtr thisPtr,
            IntPtr keyPtr,
            Il2CppMethodInfo* methodInfoPtr);

        private static INativeDetour? _detour;
        private static LoadAssetAsyncDel? _original;

        public static void Initialize()
        {
            var classType = typeof(UnityEngine.AddressableAssets.Addressables);

            var originalMethodType = classType
                .GetMethod(
                    nameof(UnityEngine.AddressableAssets.Addressables.LoadAssetAsync),
                    AccessTools.all,
                    new[] { typeof(UnityEngine.Object) })!
                .MakeGenericMethod(typeof(UnityEngine.Object));

            var methodInfoPtr = (Il2CppMethodInfo*) (IntPtr) Il2CppInteropUtils
                .GetIl2CppMethodInfoPointerFieldForGeneratedMethod(originalMethodType).GetValue(null)!;

            var methodInfo = UnityVersionHandler.Wrap(methodInfoPtr);
            var methodPtr = methodInfo.MethodPointer;

            Logger<MitochondriaResourcesPlugin>.Info(
                $"Patching {classType.Name}.{originalMethodType.Name}() at 0x{methodPtr.ToString("X")}");

            _detour = INativeDetour.CreateAndApply(methodPtr, Detour, out _original);
        }

        private static IntPtr Detour(IntPtr thisPtr, IntPtr keyPtr, Il2CppMethodInfo* methodInfoPtr)
        {
            var keyClassPtr = IL2CPP.il2cpp_object_get_class(keyPtr);

            var assetGuid = keyClassPtr == Il2CppClassPointerStore<AssetReference>.NativeClassPtr
                ? new AssetReference(keyPtr).AssetGUID
                : keyClassPtr == Il2CppClassPointerStore<string>.NativeClassPtr
                    ? IL2CPP.Il2CppStringToManaged(keyPtr)
                    : null;

            if (assetGuid != null)
            {
                var locationName = $"{Constants.AddressablesLocationNameGuidPrefix}/{assetGuid}";

                if (AllHandles.TryGetValue(locationName, out var handles))
                {
                    if (handles.OperationHandle.IsValid())
                    {
                        handles.OperationHandle.Acquire();
                        return IL2CPP.il2cpp_object_unbox(handles.OperationHandle.Pointer);
                    }

                    AllHandles.Remove(locationName);
                    handles.ResourceHandle.Dispose();
                }

                if (CustomAddressables.ResourceProviders.TryGetValue(assetGuid, out var provider))
                {
                    var resourceHandle = provider.AcquireBoxedHandle();

                    var operationHandle =
                        CachedResourceManager.CreateCompletedOperationHandle(resourceHandle.ResourceType,
                            resourceHandle.BoxedResource);

                    AllHandles.Add(locationName, new Handles(operationHandle, resourceHandle));
                    operationHandle.LocationName = locationName;

                    var operation =
                        new ResourceManager.CompletedOperation<Il2CppSystem.Object>(operationHandle.InternalOp.Pointer);

                    operation.m_OnDestroyAction += (Action<IAsyncOperation>) (
                        _ =>
                        {
                            AllHandles.Remove(operationHandle.LocationName);
                            resourceHandle.Dispose();
                        });

                    return IL2CPP.il2cpp_object_unbox(operationHandle.Pointer);
                }
            }

            return _original!.Invoke(thisPtr, keyPtr, methodInfoPtr);
        }
    }

    private record Handles(AsyncOperationHandle OperationHandle, ResourceHandle ResourceHandle);
}
