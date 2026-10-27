using HarmonyLib;
using Model;
using UI.CarInspector;
using ExampleNativeMod.Windows;

namespace ExampleNativeMod.Patches
{
    // This is a harmless example patch. It never blocks or changes the original method.
    [HarmonyPatch(typeof(CarInspector), nameof(CarInspector.Show))]
    internal static class CarInspectorPatch
    {
        [HarmonyPostfix]
        private static void AfterCarInspectorShow(Car car)
        {
            ModEntryPoint.Log($"ExampleNativeMod: inspected car {car.id}.");

            // Demo: show our native custom window on the first car inspection
            // of each world. Remove this line if you don't want the popup.
            ExampleWindow.ShowOnce();
        }
    }
}
