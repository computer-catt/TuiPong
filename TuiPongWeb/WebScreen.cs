using TuiPong;

namespace TuiPongWeb;

public class WebScreen(Action<string> callback) : ScreenBase {
    public void SetScreenBounds(int width, int height) {
        ScreenHeight = height;
        ScreenWidth = width;
        ScreenText = new char[ScreenHeight * ScreenWidth];
        Center = (ScreenHeight / 2, ScreenWidth / 2);
    }
    
    public override void UpdateScreenBounds() => Array.Clear(ScreenText);
    public override void PushDisplay(object value) => callback(value.ToString());
}