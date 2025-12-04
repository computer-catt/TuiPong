using TuiCommon;

namespace TuiPongWeb;

public class WebScreen(string[] args, Action<string> callback) : ScreenBase(args) {
    private Action<string> _callback;
    
    protected override void ShowError(object e) =>
        Console.Error.WriteLine(e);
    
    public void SetScreenBounds(int width, int height) {
        ScreenHeight = height;
        ScreenWidth = width;
        ScreenText = new char[ScreenHeight * ScreenWidth];
        Center = (ScreenHeight / 2, ScreenWidth / 2);
    }

    protected override void UpdateScreenBounds() => Array.Clear(ScreenText);
    protected override void PushDisplay(object value) => callback(value.ToString());
}