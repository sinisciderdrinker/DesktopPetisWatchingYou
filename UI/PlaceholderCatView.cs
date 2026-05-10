using System.Globalization;
using System.Windows;
using System.Windows.Media;
using MediaBrush = System.Windows.Media.Brush;
using MediaBrushes = System.Windows.Media.Brushes;
using MediaColor = System.Windows.Media.Color;
using MediaPen = System.Windows.Media.Pen;
using WpfPoint = System.Windows.Point;

namespace DesktopKitty.UI;

public sealed class PlaceholderCatView : FrameworkElement
{
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
        nameof(Label),
        typeof(string),
        typeof(PlaceholderCatView),
        new FrameworkPropertyMetadata("DesktopKitty", FrameworkPropertyMetadataOptions.AffectsRender));

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        var width = ActualWidth <= 0 ? 180 : ActualWidth;
        var height = ActualHeight <= 0 ? 180 : ActualHeight;
        var center = new WpfPoint(width / 2, height / 2 + 10);
        var bodyBrush = new SolidColorBrush(MediaColor.FromRgb(248, 196, 96));
        var linePen = new MediaPen(new SolidColorBrush(MediaColor.FromRgb(72, 55, 42)), 3);

        dc.DrawEllipse(bodyBrush, linePen, center, width * 0.28, height * 0.24);
        dc.DrawEllipse(bodyBrush, linePen, new WpfPoint(width / 2, height * 0.36), width * 0.22, height * 0.18);
        DrawEar(dc, width * 0.38, height * 0.22, width * 0.47, height * 0.10, width * 0.50, height * 0.30, bodyBrush, linePen);
        DrawEar(dc, width * 0.62, height * 0.22, width * 0.53, height * 0.10, width * 0.50, height * 0.30, bodyBrush, linePen);
        dc.DrawEllipse(MediaBrushes.Black, null, new WpfPoint(width * 0.43, height * 0.35), 3, 4);
        dc.DrawEllipse(MediaBrushes.Black, null, new WpfPoint(width * 0.57, height * 0.35), 3, 4);
        dc.DrawEllipse(MediaBrushes.DarkSalmon, null, new WpfPoint(width * 0.50, height * 0.42), 4, 3);

        var text = new FormattedText(
            Label,
            CultureInfo.CurrentCulture,
            System.Windows.FlowDirection.LeftToRight,
            new Typeface("Segoe UI"),
            12,
            new SolidColorBrush(MediaColor.FromRgb(70, 70, 70)),
            VisualTreeHelper.GetDpi(this).PixelsPerDip)
        {
            TextAlignment = TextAlignment.Center
        };

        dc.DrawText(text, new WpfPoint(width / 2 - text.Width / 2, height - text.Height - 8));
    }

    private static void DrawEar(
        DrawingContext dc,
        double x1,
        double y1,
        double x2,
        double y2,
        double x3,
        double y3,
        MediaBrush brush,
        MediaPen pen)
    {
        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(new WpfPoint(x1, y1), true, true);
            ctx.LineTo(new WpfPoint(x2, y2), true, false);
            ctx.LineTo(new WpfPoint(x3, y3), true, false);
        }

        geometry.Freeze();
        dc.DrawGeometry(brush, pen, geometry);
    }
}
