namespace TuiPong;

public abstract class ScreenHandler {
    protected int ScreenWidth;
    protected int ScreenHeight;
    protected char[,] ScreenText = new char[1,1];
    protected int RefreshSpeed = 16; // 60 FPS ish
    protected int UpdateSpeed = 16; // 60 FPS ish
    protected (int y, int x) Center;
    
    private bool _started;

    public void StartGame() {
        if (_started) return;
        _started = true;
        
        new Thread(RenderLoop).Start();
        new Thread(UpdateLoop).Start();
    }

    protected virtual void Start() {}
    protected abstract void Render();
    protected virtual void Update() {}

    private void UpdateLoop() {
        while (_started) {
            try {
                if (ScreenWidth > 0 && ScreenHeight > 0) Update();
                Thread.Sleep(UpdateSpeed);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                Thread.Sleep(UpdateSpeed * 50);
            }
        }
    }

    private void RenderLoop() {
        Console.CursorVisible = false;
        Start();
        while (_started) {
            try {
                bool didChange = ScreenHeight != Console.BufferHeight || ScreenWidth != Console.BufferHeight;
                ScreenWidth = Console.BufferWidth;
                ScreenHeight = Console.BufferHeight;
                if (didChange) ScreenText = new char[ScreenHeight,  ScreenWidth];
                else for (int i = 0; i < ScreenHeight; i++)
                        for (int j = 0; j < ScreenWidth; j++)
                            ScreenText[i, j] = ' ';
                
                Center = (ScreenHeight / 2, ScreenWidth / 2);
                
                Render();
                Thread.Sleep(RefreshSpeed);
                
                var buffer = ScreenText.ToStringBuilder();
                Console.SetCursorPosition(0,0);
                Console.Write(buffer);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                Thread.Sleep(RefreshSpeed * 50);
            }
        }
    }
    
    public void StopGame() => _started = false;


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
    public void DrawString((int x, int y) pos, string text, DrawMode drawMode = DrawMode.TopLeft) {
        string[] lines;
        string lineStr;
        int y, x;
        for (int line = 0; line < (lines = text.Split('\n')).Length; line++)
        for (int charI = 0; charI < (lineStr = lines[line]).Length; charI++) {
            switch (drawMode) {
                case DrawMode.TopLeft:
                    if (ScreenText.IsInBounds(x = pos.x + charI, y = pos.y + line)) 
                        ScreenText[y, x] = lineStr[charI];
                    break;
                case DrawMode.Center:
                    if (ScreenText.IsInBounds(x = pos.x + charI - lineStr.Length/2, y = pos.y + line - lines.Length/2)) 
                        ScreenText[y, x] = lineStr[charI];
                    break;
                case DrawMode.TopRight:
                    if (ScreenText.IsInBounds(x = pos.x - charI, y = pos.y + line))
                        ScreenText[y, x] = lineStr[lineStr.Length - charI - 1];
                    break;
            }
        }
    }
    
    public enum DrawMode {
        TopLeft,              TopRight,
        
                    Center,
        
        BottomLeft,         BottomRight,
    }
}
