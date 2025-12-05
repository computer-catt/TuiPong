using TuiCommon.Applications;

namespace TuiPong;

static class MainClass {
    public static void Main(string[] args) {
        Console.CursorVisible = false;
        Console.CancelKeyPress += (_,_) => Cleanup();
        var screen = new TerminalScreen(args);
        screen.SetApplication(new Pong(screen));
        screen.StartScreen();
        screen.EnterInputLoop();
        screen.StopScreen();
        Cleanup();
    }

    static void Cleanup() {
        Console.Clear();
        Console.CursorVisible = true;
    }
}