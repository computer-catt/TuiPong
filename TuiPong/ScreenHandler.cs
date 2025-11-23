namespace TuiPong;

public abstract class ScreenHandler {
    protected int ScreenWidth;
    protected int ScreenHeight;
    protected char[,] ScreenText = new char[1,1];
    protected int RefreshSpeed = 16; // 60 FPS ish
    private bool _started;

    public void StartRendering() {
        if (!_started) _started = true;
        else return;
        _ = GameLoop();
    }

    protected virtual void Start() {}
    protected abstract void Update();

    private char[,] _oldScreenText = new char[1,1];
    private async Task GameLoop() {
        
        Start();
        while (_started) {
            try {
                ScreenWidth = Console.BufferWidth;
                ScreenHeight = Console.BufferHeight;
                ScreenText = new char[ScreenHeight,  ScreenWidth];
                Update();

                if (!_oldScreenText.ContentEquals(ScreenText)) {
                    _oldScreenText = (char[,])ScreenText.Clone();
                    var buffer = ScreenText.ToStringBuilder();
                    Console.Clear();
                    Console.Write(buffer);
                }
                
                await Task.Delay(RefreshSpeed);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                await Task.Delay(RefreshSpeed * 50);
            }
        }
    }
    
    public void StopRendering() => _started = false;
}