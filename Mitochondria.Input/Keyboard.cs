using Mitochondria.Input.Components;
using UnityEngine;

namespace Mitochondria.Input;

public static class Keyboard
{
    public static event Action<IReadOnlyList<KeyCode>>? OnKeyCombination;

    private static KeyCombinationBehaviour? _keyCombinationBehaviour;

    internal static void Initialize(MitochondriaInputPlugin plugin)
    {
        _keyCombinationBehaviour = plugin.AddComponent<KeyCombinationBehaviour>();
        _keyCombinationBehaviour.OnKeyCombination += keyCodes => OnKeyCombination?.Invoke(keyCodes);
    }
}
