using TuiCommon;

namespace TuiCli;

public class TerminalScreen : AnsiColorBase {
    protected override void ShowError(object e) {
        Console.Clear();
        Console.Error.WriteLine(e);
        Console.ReadLine();
    }

    protected override void ClearScreen() {
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

    public override void SetScreenBounds(int width, int height, bool colors = true) {
        base.SetScreenBounds(width, height, colors);
        if (!ManualScreenwrap) Array.Fill(ScreenText, ' ');
    }

    public async Task BoundUpdateLoop(int iterationDelay = 200) {
        while (Running) {
            if (ScreenHeight != Console.BufferHeight || ScreenWidth != Console.BufferWidth)
                SetScreenBounds(Console.BufferWidth, Console.BufferHeight);
            await Task.Delay(iterationDelay);
        }
    }
}
