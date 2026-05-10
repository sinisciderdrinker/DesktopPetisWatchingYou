namespace DesktopKitty.Assets;

public sealed class CatManifestDto
{
    public string Id { get; set; } = "default";
    public string Name { get; set; } = "Default Kitty";
    public string DefaultMode { get; set; } = "cartoon";
    public double Scale { get; set; } = 1.0;
    public Dictionary<string, ModeDto> Modes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class ModeDto
{
    public string DisplayName { get; set; } = "";
    public string FallbackAnimation { get; set; } = "idle";
    public Dictionary<string, AnimationDto> Animations { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class AnimationDto
{
    public double Fps { get; set; } = 2;
    public bool Loop { get; set; } = true;
    public List<string> Frames { get; set; } = [];
}
