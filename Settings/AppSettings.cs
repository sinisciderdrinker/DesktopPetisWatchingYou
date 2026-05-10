namespace DesktopKitty.Settings;

public sealed class AppSettings
{
    public Dictionary<string, CatInstanceSettings> Cats { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public static AppSettings Default => new()
    {
        Cats = new Dictionary<string, CatInstanceSettings>(StringComparer.OrdinalIgnoreCase)
        {
            ["cat-1"] = CatInstanceSettings.Default("cartoon")
        }
    };
}

public sealed class CatInstanceSettings
{
    public string ModeId { get; set; } = "cartoon";
    public double? Left { get; set; }
    public double? Top { get; set; }

    public static CatInstanceSettings Default(string modeId)
    {
        return new CatInstanceSettings
        {
            ModeId = modeId
        };
    }
}
