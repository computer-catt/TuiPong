using System.Diagnostics;

namespace TuiCommon;

public class FrameCounter {
    private readonly Stopwatch _stopwatch = new();
    private int _totalFrames;
    private readonly int _intervalMs;
    private int _activeFrames;
    private int _lastFrames;
    private long _lastElapsedMs;
    
    public FrameCounter(int intervalMs = 1000) {
        _stopwatch.Start();
        _intervalMs = intervalMs;
    }

    public void PushNewFrame() {
        _totalFrames++;
        _activeFrames++;
        if (_lastElapsedMs + _intervalMs >= _stopwatch.ElapsedMilliseconds) return;
        _lastFrames = _activeFrames;
        _activeFrames = 0;
        _lastElapsedMs = _stopwatch.ElapsedMilliseconds;
    }

    public int GetFrames() => _totalFrames;
    public double GetFps() => _lastFrames * (1000f / _intervalMs);
}