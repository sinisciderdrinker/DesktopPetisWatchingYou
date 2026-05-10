using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DesktopKitty.UI;

public sealed class CatWindow : Window
{
    private readonly Grid _root = new();
    private readonly System.Windows.Controls.Image _image = new();
    private readonly PlaceholderCatView _placeholder = new();
    private CatMenuModel _menuModel = new("", [], []);

    public event Action? MoveRequested;
    public event Action<string>? ActionRequested;
    public event Action<string>? ModeRequested;
    public event Action? HideRequested;
    public event Action? RecallRequested;
    public event Action? QuitRequested;

    public CatWindow()
    {
        Width = 180;
        Height = 180;
        MinWidth = 96;
        MinHeight = 96;
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = System.Windows.Media.Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Topmost = true;
        Title = "DesktopKitty";

        _image.Stretch = Stretch.Uniform;
        _image.SnapsToDevicePixels = true;
        _placeholder.Visibility = Visibility.Visible;
        _root.Background = System.Windows.Media.Brushes.Transparent;
        _root.Children.Add(_placeholder);
        _root.Children.Add(_image);
        Content = _root;

        MouseLeftButtonDown += OnMouseLeftButtonDown;
        MouseRightButtonUp += (_, _) => OpenContextMenu();
        LocationChanged += (_, _) => MoveRequested?.Invoke();
    }

    public void ShowFrame(BitmapSource? frame)
    {
        if (frame is null)
        {
            _image.Source = null;
            _image.Visibility = Visibility.Collapsed;
            _placeholder.Visibility = Visibility.Visible;
            return;
        }

        _placeholder.Visibility = Visibility.Collapsed;
        _image.Visibility = Visibility.Visible;
        _image.Source = frame;
        ApplyFrameSize(frame);
    }

    public void ShowPlaceholder(string catName, string modeName)
    {
        _image.Source = null;
        _image.Visibility = Visibility.Collapsed;
        _placeholder.Visibility = Visibility.Visible;
        _placeholder.Label = $"{catName}\n{modeName}";
    }

    public void SetMenuModel(CatMenuModel menuModel)
    {
        _menuModel = menuModel;
    }

    private void ApplyFrameSize(BitmapSource frame)
    {
        const double maxSide = 220;
        var width = frame.Width;
        var height = frame.Height;
        if (width <= 0 || height <= 0)
        {
            return;
        }

        var scale = Math.Min(1.0, maxSide / Math.Max(width, height));
        Width = Math.Max(96, width * scale);
        Height = Math.Max(96, height * scale);
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState != MouseButtonState.Pressed)
        {
            return;
        }

        try
        {
            DragMove();
        }
        catch (InvalidOperationException)
        {
            // DragMove can throw if Windows has already released capture.
        }
    }

    private void OpenContextMenu()
    {
        var menu = new ContextMenu();

        var actionsMenu = new MenuItem { Header = "动作" };
        foreach (var action in _menuModel.Actions)
        {
            var item = new MenuItem { Header = ActionLabel(action), Tag = action };
            item.Click += (_, _) => ActionRequested?.Invoke(action);
            actionsMenu.Items.Add(item);
        }

        if (actionsMenu.Items.Count == 0)
        {
            actionsMenu.IsEnabled = false;
            actionsMenu.Header = "动作（当前模式暂无）";
        }

        var modesMenu = new MenuItem { Header = "切换外观" };
        foreach (var mode in _menuModel.Modes)
        {
            var item = new MenuItem
            {
                Header = mode.DisplayName,
                Tag = mode.Id,
                IsCheckable = true,
                IsChecked = string.Equals(mode.Id, _menuModel.CurrentModeId, StringComparison.OrdinalIgnoreCase)
            };
            item.Click += (_, _) => ModeRequested?.Invoke(mode.Id);
            modesMenu.Items.Add(item);
        }

        menu.Items.Add(actionsMenu);
        menu.Items.Add(modesMenu);
        menu.Items.Add(new Separator());
        menu.Items.Add(MenuItem("消失半小时", () => HideRequested?.Invoke()));
        menu.Items.Add(MenuItem("重新出现", () => RecallRequested?.Invoke()));
        menu.Items.Add(new Separator());
        menu.Items.Add(MenuItem("退出", () => QuitRequested?.Invoke()));
        menu.PlacementTarget = this;
        menu.IsOpen = true;
    }

    private static MenuItem MenuItem(string header, Action action)
    {
        var item = new MenuItem { Header = header };
        item.Click += (_, _) => action();
        return item;
    }

    private static string ActionLabel(string action)
    {
        return action.ToLowerInvariant() switch
        {
            "idle" => "待着",
            "sleep" => "睡觉",
            "walk" => "走动",
            "stretch" => "伸懒腰",
            "look" => "看看",
            "sit" => "坐下",
            _ => action
        };
    }
}
