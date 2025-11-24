namespace TuiPong;

public class Wtest : ScreenHandler {
    private int _x = 5;
    private int _y = 5;

    private string _screenText = "";
    private string _finalScreenText = "";
    
    protected override void Update() {
        if (ScreenText.IsInBounds(_x, _y)) ScreenText[_y, _x] = 'W';
        DrawString((Center.x, Center.y), _screenText, DrawMode.Center);
        DrawString((Center.x, 1 + Center.y), _finalScreenText, DrawMode.Center);
    }

    public void MoveHorizontal(int meow) => _x += meow;
    public void MoveVertical(int meow) => _y += meow;

    protected override void OnKeyRecieved(ConsoleKeyInfo keyInfo) {
        ConsoleKey key = keyInfo.Key;
        switch (key) {
            case ConsoleKey.W:
                MoveVertical(-1);
                break;
            case ConsoleKey.S:
                MoveVertical(1);
                break;
            case ConsoleKey.A:
                MoveHorizontal(-1);
                break;
            case ConsoleKey.D:
                MoveHorizontal(1);
                break;
            case ConsoleKey.F:
                GetUserInput(text => _screenText = text, finalText => _finalScreenText = finalText);
                break;
        }
    }
}