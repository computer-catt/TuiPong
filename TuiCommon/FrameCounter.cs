namespace TuiPong;

public class FrameCounter {
    private bool _running;
    private int _frames;
    private int _framesLastInterval;
    private int _framesLastLastInterval;
    private int _intervalMs;
    
    public void StartCounter(int reportIntervalMs = 1000) {
        if (_running) return;
        _running = true;
        _intervalMs = reportIntervalMs;
        _ = CounterTask();
    }

    async Task CounterTask() {
        while (_running) {
            await Task.Delay(_intervalMs);
            _framesLastLastInterval = _framesLastInterval;
            _framesLastInterval = _frames;
        }
    }
    
    public void PushNewFrame() => _frames++;
    public int GetFrames() => _frames;
    public int GetFramesSinceInterval() => _framesLastInterval - _framesLastLastInterval;
    public float GetFps() => GetFramesSinceInterval() * (1000f / _intervalMs);
    public void StopCounter() => _running = false;
}