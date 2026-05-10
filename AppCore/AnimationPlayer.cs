using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DesktopKitty.Assets;

namespace DesktopKitty.AppCore;

public sealed class AnimationPlayer : IDisposable
{
    private readonly DispatcherTimer _timer = new();
    private CatAnimation? _animation;
    private Action<BitmapSource?>? _onFrame;
    private int _frameIndex;

    public AnimationPlayer()
    {
        _timer.Tick += (_, _) => AdvanceFrame();
    }

    public void Play(CatAnimation animation, Action<BitmapSource?> onFrame)
    {
        Stop();
        _animation = animation;
        _onFrame = onFrame;
        _frameIndex = 0;

        if (animation.Frames.Count == 0)
        {
            onFrame(null);
            return;
        }

        _timer.Interval = TimeSpan.FromSeconds(1.0 / Math.Max(1, animation.Fps));
        onFrame(animation.Frames[0]);
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
        _animation = null;
        _onFrame = null;
        _frameIndex = 0;
    }

    private void AdvanceFrame()
    {
        if (_animation is null || _animation.Frames.Count == 0 || _onFrame is null)
        {
            return;
        }

        _frameIndex++;
        if (_frameIndex >= _animation.Frames.Count)
        {
            _frameIndex = _animation.Loop ? 0 : _animation.Frames.Count - 1;
            if (!_animation.Loop)
            {
                _timer.Stop();
            }
        }

        _onFrame(_animation.Frames[_frameIndex]);
    }

    public void Dispose()
    {
        Stop();
    }
}
