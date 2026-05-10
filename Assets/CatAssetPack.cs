namespace DesktopKitty.Assets;

public sealed record CatAssetPack(CatManifest Manifest, Dictionary<string, CatMode> Modes)
{
    public CatMode ModeOrDefault(string? modeId)
    {
        if (!string.IsNullOrWhiteSpace(modeId) && Modes.TryGetValue(modeId, out var selected))
        {
            return selected;
        }

        if (Modes.TryGetValue(Manifest.DefaultMode, out var defaultMode))
        {
            return defaultMode;
        }

        return Modes.Values.FirstOrDefault() ?? CatMode.Empty("cartoon", "卡通图片模式");
    }

    public static CatAssetPack Empty(string id)
    {
        var mode = CatMode.Empty("cartoon", "卡通图片模式");
        return new CatAssetPack(new CatManifest(id, id, mode.Id, 1.0), new Dictionary<string, CatMode>
        {
            [mode.Id] = mode
        });
    }
}

public sealed record CatManifest(string Id, string Name, string DefaultMode, double Scale);

public sealed record CatMode(
    string Id,
    string DisplayName,
    string FallbackAnimation,
    Dictionary<string, CatAnimation> Animations)
{
    public string? ResolveStartupAnimationName()
    {
        if (Animations.ContainsKey(FallbackAnimation))
        {
            return FallbackAnimation;
        }

        if (Animations.ContainsKey("idle"))
        {
            return "idle";
        }

        return Animations.Keys.FirstOrDefault();
    }

    public static CatMode Empty(string id, string displayName)
    {
        return new CatMode(id, displayName, "idle", new Dictionary<string, CatAnimation>(StringComparer.OrdinalIgnoreCase));
    }
}

public sealed record CatAnimation(string Name, double Fps, bool Loop, IReadOnlyList<System.Windows.Media.Imaging.BitmapSource> Frames);
