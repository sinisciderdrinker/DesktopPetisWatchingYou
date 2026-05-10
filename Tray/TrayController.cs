using System.Drawing;
using Forms = System.Windows.Forms;

namespace DesktopKitty.Tray;

public sealed class TrayController : IDisposable
{
    private readonly Forms.NotifyIcon _notifyIcon;

    public TrayController(Action recallAll, Action hideAllForThirtyMinutes, Action quit)
    {
        var menu = new Forms.ContextMenuStrip();
        menu.Items.Add("重新出现", null, (_, _) => recallAll());
        menu.Items.Add("全部消失半小时", null, (_, _) => hideAllForThirtyMinutes());
        menu.Items.Add(new Forms.ToolStripSeparator());
        menu.Items.Add("退出", null, (_, _) => quit());

        _notifyIcon = new Forms.NotifyIcon
        {
            Text = "DesktopKitty",
            Icon = SystemIcons.Application,
            ContextMenuStrip = menu,
            Visible = true
        };

        _notifyIcon.DoubleClick += (_, _) => recallAll();
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}
