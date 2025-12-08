using TuiCommon;

namespace TuiCli;

public class TerminalScreen : ScreenBase {
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
            if (!ManualScreenwrap) Array.Fill(ScreenText, ' ');
            Center = (ScreenHeight / 2, ScreenWidth / 2);
            SetDirtyOptional(); // TODO: refactor TerminalScreen bound setting to happen outside the update loop
        }
        else if (ManualScreenwrap) Array.Clear(ScreenText);
        else Array.Fill(ScreenText, ' ');
    }

    protected override void PushDisplay(object value) {
        Console.Write("\e[H");
        if (value is char[] charArray)
            Console.Write(charArray);
        else
            Console.WriteLine(value);
    }
    
    public void EnterInputLoop() {
        while (Running) {
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
