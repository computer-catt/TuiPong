namespace TuiPong;

public class LottaTextTest : ScreenHandler {
    private string _screenText = "";

    protected override void Start() => UpdateSpeed = 50;

    private int _revealedChar = 0;
    private string _revealedString = "";
    protected override void Update() {
        if (_revealedChar >= _screenText.Length) return;
        _revealedChar++;
        _revealedString = _screenText.Substring(0, _revealedChar);
    }

    protected override void Render() {
        DrawString((Center.x, Center.y), _revealedString, DrawMode.TopRight);
    }
    
    protected override void OnKeyRecieved(ConsoleKeyInfo keyInfo) {
        ConsoleKey key = keyInfo.Key;
        switch (key) {
            case ConsoleKey.A:
                _screenText +=
                    "This is a lot of text, it may even contain cats or peanuts, so be careful as you're eating it\nIm a cat writing a lot of text\nMeow mewoemwoemowemoewmoemowmoewmoewomweio keom we moewm owemoq mow emoew mow emo emowmowe moew mowemo\n";
                break;
        }
    }
}