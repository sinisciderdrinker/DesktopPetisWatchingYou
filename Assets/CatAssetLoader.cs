using System.IO;
using System.Text.Json;
using System.Windows.Media.Imaging;

namespace DesktopKitty.Assets;

public sealed class CatAssetLoader
{
    private readonly string _catsRoot;

    public CatAssetLoader()
    {
        _catsRoot = Path.Combine(AppContext.BaseDirectory, "Assets", "Cats");
    }

    public CatAssetPack Load(string catId)
    {
        var root = Path.Combine(_catsRoot, catId);
        var manifestPath = Path.Combine(root, "cat.json");
        if (!File.Exists(manifestPath))
        {
            return CatAssetPack.Empty(catId);
        }

        var json = File.ReadAllText(manifestPath);
        var manifest = JsonSerializer.Deserialize<CatManifestDto>(json, JsonOptions()) ?? new CatManifestDto();
        var modes = new Dictionary<string, CatMode>(StringComparer.OrdinalIgnoreCase);

        foreach (var (modeId, modeDto) in manifest.Modes)
        {
            var animations = LoadAnimations(root, modeDto.Animations);
            AddDiscoveredAnimations(root, modeId, animations);
            modes[modeId] = new CatMode(
                Id: modeId,
                DisplayName: string.IsNullOrWhiteSpace(modeDto.DisplayName) ? modeId : modeDto.DisplayName,
                FallbackAnimation: modeDto.FallbackAnimation,
                Animations: animations);
        }

        return new CatAssetPack(
            new CatManifest(
                Id: string.IsNullOrWhiteSpace(manifest.Id) ? catId : manifest.Id,
                Name: string.IsNullOrWhiteSpace(manifest.Name) ? catId : manifest.Name,
                DefaultMode: string.IsNullOrWhiteSpace(manifest.DefaultMode) ? modes.Keys.FirstOrDefault() ?? "cartoon" : manifest.DefaultMode,
                Scale: manifest.Scale <= 0 ? 1.0 : manifest.Scale),
            modes);
    }

    private static Dictionary<string, CatAnimation> LoadAnimations(
        string root,
        Dictionary<string, AnimationDto> animationDtos)
    {
        var result = new Dictionary<string, CatAnimation>(StringComparer.OrdinalIgnoreCase);

        foreach (var (name, dto) in animationDtos)
        {
            var frames = dto.Frames
                .Select(path => LoadFrame(Path.Combine(root, NormalizePath(path))))
                .Where(frame => frame is not null)
                .Cast<BitmapSource>()
                .ToArray();

            if (frames.Length == 0)
            {
                continue;
            }

            result[name] = new CatAnimation(name, Math.Max(1, dto.Fps), dto.Loop, frames);
        }

        return result;
    }

    private static void AddDiscoveredAnimations(
        string root,
        string modeId,
        Dictionary<string, CatAnimation> animations)
    {
        var modeDirectory = Path.Combine(root, modeId);
        if (!Directory.Exists(modeDirectory))
        {
            return;
        }

        foreach (var actionDirectory in Directory.EnumerateDirectories(modeDirectory))
        {
            var action = Path.GetFileName(actionDirectory);
            if (string.IsNullOrWhiteSpace(action) || animations.ContainsKey(action))
            {
                continue;
            }

            var frames = Directory.EnumerateFiles(actionDirectory, "*.png")
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .Select(LoadFrame)
                .Where(frame => frame is not null)
                .Cast<BitmapSource>()
                .ToArray();

            if (frames.Length == 0)
            {
                continue;
            }

            var fps = string.Equals(action, "walk", StringComparison.OrdinalIgnoreCase) ? 6 : 2;
            animations[action] = new CatAnimation(action, fps, true, frames);
        }
    }

    private static BitmapSource? LoadFrame(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = new Uri(path, UriKind.Absolute);
        image.EndInit();
        image.Freeze();
        return image;
    }

    private static string NormalizePath(string path)
    {
        return path.Replace('/', Path.DirectorySeparatorChar);
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
