namespace TuiPong;

static class MainClass {
    public static void Main(string[] args) {
        var game = new Pong();
        game.StartRendering();
        
        while (true) {
            ConsoleKey key = Console.ReadKey().Key;
            if (key == ConsoleKey.Escape) break;
            /*if (key == ConsoleKey.W) game.MoveVertical(-1);
            if (key == ConsoleKey.S) game.MoveVertical(1);

            if (key == ConsoleKey.A) game.MoveHorizontal(-1);
            if (key == ConsoleKey.D) game.MoveHorizontal(1);*/
            if (key == ConsoleKey.UpArrow) game.PaddleDown();
            if (key == ConsoleKey.DownArrow) game.PaddleUp();
        }
        /*game.StopRendering();*/
    }
}