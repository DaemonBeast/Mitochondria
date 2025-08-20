using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Mitochondria.Core.Utilities;

public static class GameUtils
{
    public static bool IsGameLoaded { get; private set; }

    [RegisterInIl2Cpp]
    internal class GameLoadWatcherBehaviour : MonoBehaviour
    {
        public GameLoadWatcherBehaviour(IntPtr ptr) : base(ptr) { }

        private void Awake()
            => IsGameLoaded = true;
    }
}
