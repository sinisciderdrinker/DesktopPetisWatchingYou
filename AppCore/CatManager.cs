using DesktopKitty.Assets;
using DesktopKitty.Settings;
using DesktopKitty.Tray;

namespace DesktopKitty.AppCore;

public sealed class CatManager : IDisposable
{
    private readonly CatAssetLoader _assetLoader = new();
    private readonly SettingsStore _settingsStore = new();
    private readonly List<CatInstance> _cats = [];
    private TrayController? _tray;
    private AppSettings _settings = AppSettings.Default;

    public void Start()
    {
        _settings = _settingsStore.Load();
        var pack = _assetLoader.Load("default");
        var cat = new CatInstance("cat-1", pack, _settings.Cats.GetValueOrDefault("cat-1"));

        cat.SettingsChanged += OnCatSettingsChanged;
        cat.RequestQuit += Shutdown;
        _cats.Add(cat);

        _tray = new TrayController(
            recallAll: RecallAll,
            hideAllForThirtyMinutes: HideAllForThirtyMinutes,
            quit: Shutdown);

        cat.Show();
    }

    private void OnCatSettingsChanged(object? sender, CatInstanceSettings settings)
    {
        if (sender is not CatInstance cat)
        {
            return;
        }

        _settings.Cats[cat.Id] = settings;
        _settingsStore.Save(_settings);
    }

    private void RecallAll()
    {
        foreach (var cat in _cats)
        {
            cat.Recall();
        }
    }

    private void HideAllForThirtyMinutes()
    {
        foreach (var cat in _cats)
        {
            cat.HideFor(TimeSpan.FromMinutes(30));
        }
    }

    private void Shutdown()
    {
        System.Windows.Application.Current.Shutdown();
    }

    public void Dispose()
    {
        foreach (var cat in _cats)
        {
            cat.SettingsChanged -= OnCatSettingsChanged;
            cat.Dispose();
        }

        _tray?.Dispose();
    }
}
