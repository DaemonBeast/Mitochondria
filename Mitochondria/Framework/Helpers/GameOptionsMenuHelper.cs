using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Mitochondria.Framework.Helpers;

public static class GameOptionsMenuHelper
{
    public static GameOptionsMenu? Current { get; internal set; }

    public static Transform? Transform => Current == null ? null : Current.transform;

    public delegate void OpenedHandler();

    public static event OpenedHandler? OnBeforeOpened;
    public static event OpenedHandler? OnAfterOpened;

    public static bool TryGetCurrent([NotNullWhen(true)] out GameOptionsMenu? gameOptionsMenu)
    {
        return (gameOptionsMenu = Current) != null;
    }

    internal static void InvokeOnBeforeOpened()
        => OnBeforeOpened?.Invoke();

    internal static void InvokeOnAfterOpened()
        => OnAfterOpened?.Invoke();
}