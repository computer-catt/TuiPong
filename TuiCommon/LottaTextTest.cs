using static TuiPong.ScreenBase;

namespace TuiPong;

public class LottaTextTest(ScreenBase screenBase) : TuiApplication(screenBase) {
    private string _screenText = "";

    protected internal override void Start() => Sb.UpdateSpeed = 50;

    private int _revealedChar = 0;
    private string _revealedString = "";
    protected internal override void Update() {
        if (_revealedChar >= _screenText.Length) return;
        _revealedChar++;
        _revealedString = _screenText.Substring(0, _revealedChar);
    }

    protected internal override void Render() {
        Sb.DrawString((Sb.Center.x, Sb.Center.y), _revealedString, DrawMode.Center);
        Sb.DrawString((0,0), "This text is in the top left");
        Sb.DrawString((Sb.ScreenWidth, 0), "This text is in the top right", DrawMode.TopRight);
        Sb.DrawString((Sb.ScreenWidth, Sb.ScreenHeight), "This text is in the bottom right", DrawMode.BottomRight);
        Sb.DrawString((0, Sb.ScreenHeight), "This text is in the bottom left", DrawMode.BottomLeft);
    }
    
    protected internal override void OnKeyReceived(ConsoleKeyInfo keyInfo) {
        ConsoleKey key = keyInfo.Key;
        switch (key) {
            case ConsoleKey.A:
                _screenText +=
                    "This is a lot of text, it may even contain cats or peanuts, so be careful as you're eating it\nIm a cat writing a lot of text\nMeow mewoemwoemowemoewmoemowmoewmoewomweio keom we moewm owemoq mow emoew mow emo emowmowe moew mowemo";
                break;
        }
    }
}