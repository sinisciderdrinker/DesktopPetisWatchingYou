using System.Windows;
using System.Windows.Threading;
using DesktopKitty.Assets;
using DesktopKitty.Settings;
using DesktopKitty.UI;

namespace DesktopKitty.AppCore;

public sealed class CatInstance : IDisposable
{
    private readonly CatAssetPack _assetPack;
    private readonly CatWindow _window;
    private readonly AnimationPlayer _animationPlayer = new();
    private readonly DispatcherTimer _idleTimer = new();
    private readonly DispatcherTimer _hiddenTimer = new();
    private CatMode _currentMode;
    private CatInstanceSettings _settings;

    public string Id { get; }
    public event EventHandler<CatInstanceSettings>? SettingsChanged;
    public event Action? RequestQuit;

    public CatInstance(string id, CatAssetPack assetPack, CatInstanceSettings? settings)
    {
        Id = id;
        _assetPack = assetPack;
        _settings = settings ?? CatInstanceSettings.Default(assetPack.Manifest.DefaultMode);
        _currentMode = _assetPack.ModeOrDefault(_settings.ModeId);
        _window = new CatWindow();
        _window.MoveRequested += SaveWindowPosition;
        _window.ActionRequested += PlayAction;
        _window.ModeRequested += SwitchMode;
        _window.HideRequested += () => HideFor(TimeSpan.FromMinutes(30));
        _window.RecallRequested += Recall;
        _window.QuitRequested += () => RequestQuit?.Invoke();

        _idleTimer.Interval = TimeSpan.FromSeconds(12);
        _idleTimer.Tick += (_, _) => PlayRandomAvailableAction();

        _hiddenTimer.Tick += (_, _) => Recall();
    }

    public void Show()
    {
        _window.SetMenuModel(BuildMenuModel());
        RestorePosition();
        _window.Show();
        PlaySafeStartupAnimation();
        _idleTimer.Start();
    }

    public void HideFor(TimeSpan duration)
    {
        _idleTimer.Stop();
        _animationPlayer.Stop();
        _window.Hide();
        _hiddenTimer.Stop();
        _hiddenTimer.Interval = duration;
        _hiddenTimer.Start();
    }

    public void Recall()
    {
        _hiddenTimer.Stop();
        if (!_window.IsVisible)
        {
            _window.Show();
        }

        PlaySafeStartupAnimation();
        _idleTimer.Start();
    }

    private void PlaySafeStartupAnimation()
    {
        var action = _currentMode.ResolveStartupAnimationName();
        if (action is null)
        {
            _animationPlayer.Stop();
            _window.ShowPlaceholder(_assetPack.Manifest.Name, _currentMode.DisplayName);
            return;
        }

        PlayAction(action);
    }

    private void PlayAction(string action)
    {
        if (!_currentMode.Animations.TryGetValue(action, out var animation))
        {
            return;
        }

        _animationPlayer.Play(animation, _window.ShowFrame);
    }

    private void PlayRandomAvailableAction()
    {
        var candidates = _currentMode.Animations.Keys
            .Where(name => !string.Equals(name, "walk", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (candidates.Length == 0)
        {
            return;
        }

        PlayAction(candidates[Random.Shared.Next(candidates.Length)]);
    }

    private void SwitchMode(string modeId)
    {
        if (!_assetPack.Modes.TryGetValue(modeId, out var mode))
        {
            return;
        }

        _currentMode = mode;
        _settings.ModeId = mode.Id;
        SettingsChanged?.Invoke(this, _settings);
        _window.SetMenuModel(BuildMenuModel());
        PlaySafeStartupAnimation();
    }

    private CatMenuModel BuildMenuModel()
    {
        return new CatMenuModel(
            CurrentModeId: _currentMode.Id,
            Modes: _assetPack.Modes.Values.Select(mode => new CatModeMenuItem(mode.Id, mode.DisplayName)).ToArray(),
            Actions: _currentMode.Animations.Keys.OrderBy(name => name).ToArray());
    }

    private void RestorePosition()
    {
        if (_settings.Left is not null && _settings.Top is not null)
        {
            _window.Left = _settings.Left.Value;
            _window.Top = _settings.Top.Value;
            return;
        }

        var workArea = SystemParameters.WorkArea;
        _window.Left = workArea.Right - _window.Width - 120;
        _window.Top = workArea.Bottom - _window.Height - 24;
    }

    private void SaveWindowPosition()
    {
        _settings.Left = _window.Left;
        _settings.Top = _window.Top;
        SettingsChanged?.Invoke(this, _settings);
    }

    public void Dispose()
    {
        _idleTimer.Stop();
        _hiddenTimer.Stop();
        _animationPlayer.Dispose();
        _window.Close();
    }
}
