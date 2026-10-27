# Railroader Native DLL Mod Example

A small example project for the native DLL loader introduced with Railroader 0.6 / Maintenance of Way.

It's intentionally more than an empty DLL. It shows the native entry point, world lifecycle, logging, Harmony, a host-only `MonoBehaviour`, a native custom window, and the Debug/Release build pipeline. No UMM dependency.

**This is source code, not a prebuilt mod.** You need the game installed locally to compile against its assemblies. The example targets `net472` with C# 9.

## Get it building

1. Copy `Paths.user.example` to `Paths.user` in the repository root.
2. Edit `GameDir` to the folder containing `Railroader.exe`. The targets look for `Railroader_Data\Managed` beneath it. Override `GameManagedDir` if your game layout is different.
3. Open the folder in VSCode or double click the `ExampleNativeMod.sln` to open in the default IDE (usually Visual Studio).
   If prompted, click `Trust the folder`.
4. Enable **Preferences > Features > Experimental > Allow Unsafe DLL Mods** in Railroader and restart the game. Development packs are loaded from `Documents\Railroader\ModDev` (or the actual redirected Documents folder).

The Debug target deploys to `Documents\Railroader\ModDev\ExampleNativeMod` and prints the exact path. The build first renders the manifest into `ExampleNativeMod/bin/Debug/Info.json`, then copies that generated file to `ModDev`. The source `ExampleNativeMod/Info.json` deliberately keeps its placeholders. You can override `NativeModDevDir` in `Paths.user`.

## What the example actually does

- `ModEntryPoint.cs` implements `IModEntryPoint` and `IWorldLifecycle`. It initializes Harmony on mod load and undoes its own patches on mod unload.
- `Runtime/ExampleRuntime.cs` creates a host-only GameObject on world load, logs once every 60 seconds while the host, and is destroyed on unload.
- `Patches/CarInspectorPatch.cs` adds a **postfix** to `UI.CarInspector.CarInspector.Show(Model.Car)` that logs the car ID; it doesn't cancel or modify the original method.
- `Windows/ExampleWindow.cs` uses `WindowManager` / `CustomWindowDefinition` to open a native informational window the first time a car is inspected in a world. Remove `ExampleWindow.ShowOnce()` from the patch if you don't want that popup.
- There is no UMM settings implementation. Use whatever settings approach suits your mod.

Note: `OnWorldLoad` fires on **every peer**. This example only creates its gameplay runtime if `StateManager.IsHost` is true, and also guards its `Update()` method.

## Release

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Build.ps1 -Configuration Release
powershell -ExecutionPolicy Bypass -File .\scripts\Check-Release.ps1
```

The build stages **all output files** in repository-level `bin`, not `ExampleNativeMod/bin`, and produces:

```text
bin/
├── ExampleNativeMod/
│   ├── ExampleNativeMod.dll
│   ├── 0Harmony.dll
│   └── Info.json
└── ExampleNativeMod_0.1.0.zip
```

The versioned ZIP contains the `ExampleNativeMod` directory and everything inside it. This matches the conventional extract-to-`Mods` workflow.

The `Lib.Harmony` NuGet package supplies `0Harmony.dll`, so there's no need to modify the game's `Managed` directory. Harmony is **not** included in the manifest's `assemblies` array; only `ExampleNativeMod.dll` is.

`Directory.Build.targets` collects output files generally instead of copying only three hardcoded artifacts. This matters if you later add dependencies or resources. The targets render `Info.json` using `scripts/Render-Manifest.ps1` and clean up older versioned release ZIPs.

For a build without deploying or publishing, add `-NoDeploy` to `Build.ps1`, or run `dotnet build -p:PublishMod=false`.

## Turn it into your mod

- Change `AssemblyName` and `RootNamespace` in `.csproj`, update namespaces and the solution name/path if you rename the project.
- Set a unique `identifier`, proper name/author and description in `Info.json`. Don't ship the sample ID.
- Change the version in `Directory.Build.props` (`<Version>0.1.0</Version>`).
- Change the DLL name/path checked by `Check-Release.ps1` if you rename the project.
- Replace the inspector example patch with your own code, and remove the demo window/runtime if you don't need them.
- Keep `Railroader.Modding` as your entry-point reference; other game APIs (such as the inspector and `StateManager`) are not a stable supported interface and may change in later game releases.
- Update the `rr/game` minimum version when compiling against newer game builds.

## Files

```text
ExampleNativeMod.sln
Directory.Build.props             Shared compiler/version properties
Directory.Build.targets           Game references, Debug deploy, Release ZIP
Paths.user.example                Local game path template (don't commit Paths.user)
ExampleNativeMod/
  ExampleNativeMod.csproj         Railroader, Unity, and NuGet references
  Info.json                       Native manifest template
  ModEntryPoint.cs                Mod/world lifecycle and Harmony setup
  Patches/CarInspectorPatch.cs    Sample Harmony postfix
  Runtime/ExampleRuntime.cs      Host-only tick loop
  Windows/ExampleWindow.cs       Native WindowManager window
scripts/
  Build.ps1                       Build Debug or Release
  Render-Manifest.ps1             Fill manifest placeholders
  Check-Release.ps1               Check folder and ZIP contents
```

## Limits

This package has had its source/layout checks run, and has been compiled and loaded in-game.

## Futher Reading
See the documentation: https://docs.railroadergame.com/modding/dll-mods/.
