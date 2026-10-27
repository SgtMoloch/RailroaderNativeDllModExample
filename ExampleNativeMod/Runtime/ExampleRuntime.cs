using Game.State;
using UnityEngine;

namespace ExampleNativeMod.Runtime
{
    // Optional MonoBehaviour example. Created on world load, host only.
    public sealed class ExampleRuntime : MonoBehaviour
    {
        private float _elapsed;

        private void Update()
        {
            // Leave this guard in place if authority can change while the world is loaded.
            if (!StateManager.IsHost)
                return;

            _elapsed += Time.deltaTime;
            if (_elapsed < 60f)
                return;

            _elapsed = 0f;
            ModEntryPoint.Log("ExampleNativeMod: host runtime still running.");
        }
    }
}
