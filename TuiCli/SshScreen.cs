namespace TuiCli;

public class SshScreen : AnsiColorBase {
    private Action<object>? _callback;

    public void SetCallback(Action<object> callback) => _callback = callback;
    
    protected override void ShowError(object e) =>
        Console.Error.WriteLine(e);
    
    public void SetScreenBounds(int width, int height) {
        ScreenHeight = height;
        ScreenWidth = width;
        int size = ScreenHeight * ScreenWidth;
        ScreenText        = new char [size];
        BackgroundColors  = new byte?[size];
        ForegroundColors  = new byte?[size];
        RefreshCharBuffer = new bool [size];
        Center = (ScreenHeight / 2, ScreenWidth / 2);
        SetDirtyOptional();
    }

    protected override void UpdateScreenBounds() => ClearBuffers();
    protected override void PushDisplay(object value) => _callback?.Invoke(value);
}