using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Mitochondria.Utilities;

public static class GameUtilities
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
