namespace TuiPong;

static class MainClass {
    public static void Main(string[] args) {
        var game = new Wtest();
        game.StartRendering();
        game.EnterInputLoop();
        game.StopRendering();
    }
}