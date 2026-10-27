using UI.Windows;
using UnityEngine;

namespace ExampleNativeMod.Windows
{
    internal static class ExampleWindow
    {
        private const string Identifier = "example.native-dll-mod.demo-window";
        private static bool _shownThisWorld;

        internal static void Reset() => _shownThisWorld = false;

        internal static void ShowOnce()
        {
            if (_shownThisWorld)
                return;

            WindowManager manager = WindowManager.Shared;
            if (manager == null)
                return;

            if (!manager.HasDefinition(Identifier))
            {
                manager.RegisterCustomWindow(new CustomWindowDefinition
                {
                    Identifier = Identifier,
                    Title = "Native DLL Mod Example",
                    Config = WindowConfig.Fixed(new Vector2Int(440, 180)),
                    BuilderAction = builder =>
                    {
                        builder.AddTitle("Hello from a native DLL mod!", "Example window");
                        builder.AddLabel("This window is registered with Railroader's WindowManager.");
                        builder.AddLabel("The example Harmony patch opened it the first time you inspected a car.");
                    }
                });
            }

            manager.ShowWindow(Identifier);
            _shownThisWorld = true;
        }
    }
}
