namespace TuiPong;

static class MainClass {
    public static void Main(string[] args) {
        Console.CursorVisible = false;
        var screen = new TerminalScreen();
        screen.SetApplication(new Pong(screen));
        screen.StartGame();
        screen.EnterInputLoop();
        screen.StopGame();
    }
}