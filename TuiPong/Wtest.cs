namespace TuiPong;

public class Wtest : ScreenHandler {
    private int X = 5;
    private int Y = 5;
    
    protected override void Update() {
        if (ScreenText.IsInBounds(X, Y)) ScreenText[Y, X] = 'W';
    }

    public void MoveHorizontal(int meow) => X += meow;
    public void MoveVertical(int meow) => Y += meow;
}