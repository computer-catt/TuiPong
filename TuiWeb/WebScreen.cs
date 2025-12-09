using TuiCommon;

namespace TuiWeb;

public class WebScreen(Action<string> callback) : ScreenBase {
    protected override void ShowError(object e) =>
        Console.Error.WriteLine(e);
    
    public void SetScreenBounds(int width, int height) {
        ScreenHeight = height;
        ScreenWidth = width;
        int size = ScreenHeight * ScreenWidth;
        ScreenText        = new char [size];
        /*BackgroundColors  = new byte?[size];
        ForegroundColors  = new byte?[size];
        RefreshCharBuffer = new bool [size];*/
        Center = (ScreenHeight / 2, ScreenWidth / 2);
        SetDirtyOptional();
    }

    protected override void UpdateScreenBounds() => Array.Clear(ScreenText);
    protected override void PushDisplay(object value) => callback(value.ToString() ?? "Cant convert to string");
}