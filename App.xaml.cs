using System.Windows;
using DesktopKitty.AppCore;

namespace DesktopKitty;

public partial class App : System.Windows.Application
{
    private CatManager? _catManager;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _catManager = new CatManager();
        _catManager.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _catManager?.Dispose();
        base.OnExit(e);
    }
}
