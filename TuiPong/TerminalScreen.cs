using TuiCommon;

namespace TuiPong;

public class TerminalScreen : ScreenBase {
    public override void UpdateScreenBounds() {
        bool didChange = ScreenHeight != Console.BufferHeight || ScreenWidth != Console.BufferWidth;
        if (didChange) {
            ScreenWidth = Console.BufferWidth;
            ScreenHeight = Console.BufferHeight;
            ScreenText = new char[ScreenHeight * ScreenWidth];
            Center = (ScreenHeight / 2, ScreenWidth / 2);
        }
        else Array.Clear(ScreenText);
    }

    public override void PushDisplay(object value) {
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
