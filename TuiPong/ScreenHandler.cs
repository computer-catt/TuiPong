namespace TuiPong;

public abstract class ScreenHandler {
    protected int ScreenWidth;
    protected int ScreenHeight;
    protected char[,] ScreenText = new char[1,1];
    protected int RefreshSpeed = 16; // 60 FPS ish
    protected (int y, int x) Center;
    
    private bool _started;

    public void StartRendering() {
        if (!_started) _started = true;
        else return;
        _ = GameLoop();
    }

    protected virtual void Start() {}
    protected abstract void Update();

    private char[,] _oldScreenText = new char[1,1];
    private async Task GameLoop() {
        Console.CursorVisible = false;
        Start();
        while (_started) {
            try {
                ScreenWidth = Console.BufferWidth;
                ScreenHeight = Console.BufferHeight;
                ScreenText = new char[ScreenHeight,  ScreenWidth];
                Center = (ScreenHeight / 2, ScreenWidth / 2);
                
                Update();
                
                if (!_oldScreenText.ContentEquals(ScreenText)) {
                    _oldScreenText = (char[,])ScreenText.Clone();
                    var buffer = ScreenText.ToStringBuilder();
                    Console.SetCursorPosition(0,0);
                    _ = Console.Out.WriteAsync(buffer);
                }
                
                await Task.Delay(RefreshSpeed);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                await Task.Delay(RefreshSpeed * 50);
            }
        }
    }
    
    public void StopRendering() => _started = false;

    public void DrawString((int x, int y) pos, string text, DrawMode drawMode) {
        string[] lines;
        string lineStr;
        for (int line = 0; line < (lines = text.Split('\n')).Length; line++)
            for (int charI = 0; charI < (lineStr = lines[line]).Length; charI++)
                ScreenText[pos.y + line, pos.x + charI] = lineStr[charI];
    }

    protected virtual void OnKeyRecieved(ConsoleKeyInfo key) {}

    private string _currentInput;
    private Action<string>? currentTextCallback;
    private Action<string> finalString;

    public void EnterInputLoop() {
        while (true) {
            try {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true); 
                ConsoleKey key = keyInfo.Key;
                if (key == ConsoleKey.Escape) break;
                if (currentTextCallback != null) {
                    if (key == ConsoleKey.Backspace) 
                        currentTextCallback(_currentInput = _currentInput.Substring(0, _currentInput.Length - 1));
                    if (key != ConsoleKey.Enter) {
                        if (keyInfo.KeyChar == 32 || keyInfo.KeyChar >= 65 && keyInfo.KeyChar <= 122)
                            currentTextCallback(_currentInput += keyInfo.KeyChar);
                        continue;
                    }
                    finalString(_currentInput);
                    currentTextCallback = null;
                    _currentInput = "";
                    continue;
                }
                OnKeyRecieved(keyInfo);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }

    protected void GetUserInput(Action<string> currentTextCallback, Action<string> finalString) {
        this.currentTextCallback = currentTextCallback;
        this.finalString = finalString;
    }
    
    public enum DrawMode {
        Center,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }
}