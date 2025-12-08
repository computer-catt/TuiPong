using TuiCommon;

namespace TuiCli;

public class SshScreen : ScreenBase {
    private Action<string>? _callback;

    public void SetCallback(Action<string> callback) => _callback = callback;
    
    protected override void ShowError(object e) =>
        Console.Error.WriteLine(e);
    
    public void SetScreenBounds(int width, int height) {
        ScreenHeight = height;
        ScreenWidth = width;
        ScreenText = new char[ScreenHeight * ScreenWidth];
        Center = (ScreenHeight / 2, ScreenWidth / 2);
        SetDirtyOptional();
    }

    protected override void UpdateScreenBounds() => Array.Clear(ScreenText);
    protected override void PushDisplay(object value) => _callback?.Invoke(((char[])value).ToStringBuilder().ToString());
}