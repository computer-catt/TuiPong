namespace TuiPong;

static class MainClass {
    public static void Main(string[] args) {
        var game = new Pong();
        game.StartGame();
        game.EnterInputLoop();
        game.StopGame();
    }
}