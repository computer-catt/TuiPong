using TuiCommon;

namespace TuiCli;

public class TerminalScreen : AnsiColorBase {
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
            int size = ScreenHeight * ScreenWidth;
            ScreenText        = new char [size];
            BackgroundColors  = new byte?[size];
            ForegroundColors  = new byte?[size];
            RefreshCharBuffer = new bool [size];
            if (!ManualScreenwrap) Array.Fill(ScreenText, ' ');
            Center = (ScreenHeight / 2, ScreenWidth / 2);
            SetDirtyOptional(); // TODO: refactor TerminalScreen bound setting to happen outside the update loop
        }
        
        if (ManualScreenwrap) 
            ClearBuffers();
        else 
            ClearBuffers(true);
    }

    protected override void PushDisplay(object value) {
        Console.Write("\e[H");
        if (value is char[] charArray)
            Console.Write(charArray);
        else
            Console.Write(value);
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
