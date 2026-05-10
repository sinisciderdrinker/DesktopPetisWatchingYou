# 桌宠正在看着你
# DesktopPet is Watching You

灵感来自 [DockCat](https://github.com/Auwuua/DockCat)。
Inspired by [DockCat](https://github.com/Auwuua/DockCat).

这是一个 Windows 桌面小猫原型程序。
This is a Windows desktop kitty prototype.

它由 Codex 协助开发，用于学习和实验桌面宠物的基础实现。
It was built with help from Codex as a learning and experimental desktop pet project.

## 当前功能
## Current Features

透明无边框小猫窗口。
Transparent borderless kitty window.

可以用鼠标左键拖动小猫。
Drag the kitty with the left mouse button.

支持右键菜单交互。
Right-click menu interactions are supported.

支持真猫图片模式和卡通图片模式切换。
Supports switching between photo mode and cartoon mode.

支持让小猫消失半小时。
Supports hiding the kitty for 30 minutes.

支持系统托盘菜单，用于重新出现、全部隐藏和退出。
Includes a tray menu for recall, hiding all kitties, and quitting.

代码结构预留了多猫咪扩展能力。
The code structure is prepared for future multi-kitty support.

## 资源结构
## Asset Layout

把 PNG 动作帧放在 `Assets/Cats/<cat-id>/<mode>/<action>/` 下面。
Put PNG animation frames under `Assets/Cats/<cat-id>/<mode>/<action>/`.

例如：
For example:

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

如果当前模式没有某个动作，它不会出现在右键菜单里。
If an action does not exist in the current mode, it will not appear in the context menu.

如果当前模式没有某个动作，自动模式也不会触发它。
If an action does not exist in the current mode, automatic behavior will not trigger it either.

## 构建
## Build

在项目目录中运行：
Run this from the project directory:

```powershell
dotnet build
```

## 运行
## Run

需要安装 .NET 8 SDK。
The .NET 8 SDK is required.

在项目目录中运行：
Run this from the project directory:

```powershell
dotnet run
```

如果你在上一级目录，也可以指定项目文件运行：
If you are in the parent directory, you can run with the project file:

```powershell
dotnet run --project DesktopKitty\DesktopKitty.csproj
```

## 备注
## Notes

这是一个早期原型，不保证稳定性。
This is an early prototype and is not guaranteed to be stable.

请自行承担使用风险。
Use at your own risk.
