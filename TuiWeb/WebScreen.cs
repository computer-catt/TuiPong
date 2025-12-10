using TuiCommon;

namespace TuiWeb;

public class WebScreen(Action<string> callback) : ScreenBase {
    protected override void ShowError(object e) =>
        Console.Error.WriteLine(e);

    /// <param name="colors">dont set this it does nothing :3</param>
    public override void SetScreenBounds(int width, int height, bool colors = true) {
        base.SetScreenBounds(width, height, false);
    }

    protected override void ClearScreen() => Array.Clear(ScreenText);
    protected override void PushDisplay(object value) => callback(value.ToString() ?? "Cant convert to string");
}