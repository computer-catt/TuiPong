using TuiCommon;

namespace TuiPong;

public class TerminalScreen(string[] args) : ScreenBase(args) {
    protected override void ShowError(object e) {
        Console.Clear();
        Console.Error.WriteLine(e);
        Console.ReadLine();
    }

    protected override void UpdateScreenBounds() {
        bool didChange = ScreenHeight != Console.BufferHeight || ScreenWidth != Console.BufferWidth;
        if (didChange) {
            ScreenWidth = Console.BufferWidth;
            ScreenHeight = Console.BufferHeight;
            ScreenText = new char[ScreenHeight * ScreenWidth];
            Center = (ScreenHeight / 2, ScreenWidth / 2);
        }
        else Array.Clear(ScreenText);
    }

    protected override void PushDisplay(object value) {
        Console.Write("\e[H");
        Console.Write(value);
    }
    
    public void EnterInputLoop() {
        while (true) {
            try {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                TuiKey key = new(keyInfo);
                SendKey(key);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
