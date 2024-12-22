using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Mitochondria.Input.Components;

[RegisterInIl2Cpp]
public class KeyCombinationBehaviour : MonoBehaviour
{
    [HideFromIl2Cpp]
    public event Action<IReadOnlyList<KeyCode>>? OnKeyCombination;

    private readonly KeyCode[] _allKeyCodes = Enum
        .GetValues<KeyCode>()
        .Except(
            new[]
            {
                KeyCode.None, KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2, KeyCode.Mouse3, KeyCode.Mouse4,
                KeyCode.Mouse5, KeyCode.Mouse6
            })
        .ToArray();

    private KeyCode[] _lastKeysDown = Array.Empty<KeyCode>();
    private bool _preventTrigger;

    public KeyCombinationBehaviour(IntPtr ptr) : base(ptr)
    {
    }

    private void Update()
    {
        // Terribly inefficient but very robust (I think)

        var keysDown = _allKeyCodes.Where(UnityEngine.Input.GetKey).ToArray();

        if (keysDown.Length < _lastKeysDown.Length && !_preventTrigger)
        {
            _preventTrigger = true;
            OnKeyCombination?.Invoke(_lastKeysDown);
        }
        else if (keysDown.Length > _lastKeysDown.Length)
        {
            _preventTrigger = false;
        }

        _lastKeysDown = keysDown;
    }
}
