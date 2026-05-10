namespace DesktopKitty.UI;

public sealed record CatMenuModel(
    string CurrentModeId,
    IReadOnlyList<CatModeMenuItem> Modes,
    IReadOnlyList<string> Actions);

public sealed record CatModeMenuItem(string Id, string DisplayName);
