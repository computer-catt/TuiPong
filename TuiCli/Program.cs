using TuiCommon.Applications;

namespace TuiCli;

static class MainClass {
    public static void Main(string[] args) {
        if (args.Length != 0 && args[0].ToLowerInvariant() is "sshinit" or "briissh") 
            SshServerLoader.Init(args);
        
        else InitScreen(args);
    }

    public static void InitScreen(string[] args) {
        Console.CursorVisible = false;
        Console.CancelKeyPress += (_,_) => Cleanup();
        
        var screen = new TerminalScreen();
        screen.SetArgs(args);
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