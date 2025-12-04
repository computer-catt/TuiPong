using static TuiCommon.ScreenBase;

namespace TuiCommon.Applications;

public class Wtest(ScreenBase screenBase) : TuiApplication(screenBase) {
    private int _x = 5;
    private int _y = 5;

    private string _screenText = "";
    private string _finalScreenText = "";
    
    protected internal override void Render() {
        if (Extensions.IsInBounds(Sb.ScreenHeight, Sb.ScreenWidth, _x, _y)) Sb.DrawChar(_x, _y, 'W');
        Sb.DrawString((Sb.Center.x, Sb.Center.y), $"{_x} {_y} {Sb.ResolveCharPos(_x, _y)}");
        Sb.DrawString((Sb.Center.x, Sb.Center.y), _screenText, DrawMode.Center);
        Sb.DrawString((Sb.Center.x, 1 + Sb.Center.y), _finalScreenText, DrawMode.Center);
    }

    public void MoveHorizontal(int meow) => _x += meow;
    public void MoveVertical(int meow) => _y += meow;

    protected internal override void OnKeyReceived(TuiKey key) {
        switch (key.Key.ToLower()) {
            case "w":
                MoveVertical(-1);
                break;
            case "s":
                MoveVertical(1);
                break;
            case "a":
                MoveHorizontal(-1);
                break;
            case "d":
                MoveHorizontal(1);
                break;
            case "f":
                Sb.GetUserInput(text => _screenText = text, finalText => _finalScreenText = finalText);
                break;
        }
    }
}