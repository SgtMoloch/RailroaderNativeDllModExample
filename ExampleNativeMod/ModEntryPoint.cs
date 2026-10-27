using System;
using HarmonyLib;
using Game.State;
using Railroader.Modding;
using UnityEngine;
using ExampleNativeMod.Runtime;
using ExampleNativeMod.Windows;

namespace ExampleNativeMod
{
    // Native Railroader entry point. No UMM, no special registration method.
    public sealed class ModEntryPoint : IModEntryPoint, IWorldLifecycle
    {
        private static IModContext? _context;
        private Harmony? _harmony;
        private GameObject? _runtimeObject;
        private bool _worldLoaded;

        internal static void Log(string message) => _context?.Log.Info(message);

        public void OnModLoad(IModContext context)
        {
            // Assemblies and static fields survive disable/re-enable.
            _context = context;
            _runtimeObject = null;
            _worldLoaded = false;
            ExampleWindow.Reset();

            Log("ExampleNativeMod: mod loaded.");
            _harmony = new Harmony(context.ModIdentifier);
            try
            {
                _harmony.PatchAll(typeof(ModEntryPoint).Assembly);
                Log("ExampleNativeMod: Harmony patches installed.");
            }
            catch
            {
                _harmony.UnpatchAll(context.ModIdentifier);
                _harmony = null;
                _context = null;
                throw;
            }
        }

        public void OnWorldLoad()
        {
            _worldLoaded = true;
            ExampleWindow.Reset();
            Log("ExampleNativeMod: world loaded.");

            // The world callbacks run on clients too. Only the host gets a runtime.
            if (!StateManager.IsHost || _runtimeObject != null)
                return;

            _runtimeObject = new GameObject("ExampleNativeMod.Runtime");
            UnityEngine.Object.DontDestroyOnLoad(_runtimeObject);
            _runtimeObject.AddComponent<ExampleRuntime>();
        }

        public void OnWorldUnload()
        {
            if (!_worldLoaded && _runtimeObject == null)
                return;

            _worldLoaded = false;
            ExampleWindow.Reset();
            if (_runtimeObject != null)
            {
                UnityEngine.Object.Destroy(_runtimeObject);
                _runtimeObject = null;
            }

            Log("ExampleNativeMod: world unloaded.");
        }

        public void OnModUnload()
        {
            OnWorldUnload();
            if (_harmony != null)
            {
                if (_context != null)
                    _harmony.UnpatchAll(_context.ModIdentifier);
                _harmony = null;
            }

            Log("ExampleNativeMod: mod unloaded.");
            _context = null;
        }
    }
}
