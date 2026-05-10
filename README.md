# DesktopKitty

Windows desktop kitty prototype.

## What works in this first version

- Transparent borderless kitty window
- Drag the kitty with the left mouse button
- Right-click menu
- Switch between photo and cartoon modes
- Menu actions are generated only from the current mode's available animations
- Hide for 30 minutes
- Tray menu with recall and quit
- Multi-kitty ready structure, currently creating one kitty instance

## Asset layout

Put PNG frames under `Assets/Cats/<cat-id>/<mode>/<action>/`.

Example:

```text
Assets/Cats/default/
  cat.json
  photo/
    idle/
      idle_01.png
  cartoon/
    idle/
      idle_01.png
      idle_02.png
    walk/
      walk_01.png
      walk_02.png
```

If an action does not exist in the current mode, it will not appear in the right-click menu and automatic behavior will not trigger it.

The loader also discovers PNGs automatically from folders. For quick testing, you can simply create:

```text
Assets/Cats/default/cartoon/idle/idle_01.png
Assets/Cats/default/cartoon/sleep/sleep_01.png
Assets/Cats/default/cartoon/walk/walk_01.png
Assets/Cats/default/cartoon/walk/walk_02.png
```

Those folder names become action names.

This machine currently has .NET runtimes but no .NET SDK, so build with Visual Studio or install the .NET SDK and run:

```powershell
dotnet build
dotnet run --project DesktopKitty.csproj
```

Recommended SDK: .NET 8 SDK with Windows Desktop workload support.
# DesktopPet
