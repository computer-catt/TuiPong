namespace TuiPong;

public abstract class ScreenHandler {
    protected int ScreenWidth;
    protected int ScreenHeight;
    protected char[] ScreenText = new char[1];
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

    public int ResolveCharPos(int x, int y) => y * ScreenWidth + x;
    protected void DrawChar(int x, int y, char ch) => ScreenText[ResolveCharPos(x,y)] = ch;
    
    private void RenderLoop() {
        Console.CursorVisible = false;
        Start();
        while (_started) {
            try {
                bool didChange = ScreenHeight != Console.BufferHeight || ScreenWidth != Console.BufferWidth;
                if (didChange) {
                    ScreenWidth = Console.BufferWidth;
                    ScreenHeight = Console.BufferHeight;
                    ScreenText = new char[ScreenHeight * ScreenWidth];
                    Center = (ScreenHeight / 2, ScreenWidth / 2);
                }
                else Array.Clear(ScreenText);
                
                Render();
                Thread.Sleep(RefreshSpeed);
                
                var buffer = ScreenText.ToStringBuilder();
                Console.Write("\e[H");
                Console.Write(buffer);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                Thread.Sleep(RefreshSpeed * 50);
            }
        }
    }
    
    public void StopGame() => _started = false;


    protected virtual void OnKeyReceived(ConsoleKeyInfo key) {}

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
                OnKeyReceived(keyInfo);
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

    private void ParseFullDrawString((int x, int y) pos, string text, DrawMode drawMode = DrawMode.Center) {
        string[] lines;
        string lineStr;
        int y, x;
        for (int line = 0; line < (lines = text.Split('\n')).Length; line++)
        for (int charI = 0; charI < (lineStr = lines[line]).Length; charI++)
            switch (drawMode) {
                case DrawMode.Center:
                    if (Extensions.IsInBounds(ScreenHeight, ScreenWidth, x = pos.x + charI - lineStr.Length / 2, y = pos.y + line - lines.Length / 2))
                        DrawChar(x, y, lineStr[charI]);
                    break;
                case DrawMode.TopRight:
                    if (Extensions.IsInBounds(ScreenHeight, ScreenWidth, x = pos.x - charI - 1, y = pos.y + line))
                        DrawChar(x, y, lineStr[lineStr.Length - charI - 1]);
                    break;
                case DrawMode.BottomRight:
                    if (Extensions.IsInBounds(ScreenHeight, ScreenWidth, x = pos.x - charI - 1, y = pos.y + line - lines.Length))
                        DrawChar(x, y, lineStr[lineStr.Length - charI - 1]);
                    break;
            }
    }

    private void ParseLessDrawString((int x, int y) pos, string text, DrawMode drawMode = DrawMode.TopLeft) {
        int y, x, charPos = 0, line = 0;
        for (int charI = 0; charI < text.Length; charI++) {
            if (text[charI] == '\n') { line++; charPos = 0; }

            switch (drawMode) {
                case DrawMode.TopLeft:
                    if (Extensions.IsInBounds(ScreenHeight, ScreenWidth, x = pos.x + charPos, y = pos.y + line)) 
                        DrawChar(x, y, text[charI]);
                    break;
                case DrawMode.BottomLeft:
                    int lines = text.Count('\n') + 1;
                    if (Extensions.IsInBounds(ScreenHeight, ScreenWidth, x = pos.x + charPos, y = pos.y + line - lines)) 
                        DrawChar(x, y, text[charI]);
                    break;
            }

            charPos++;
        }
    }

    public void DrawString((int x, int y) pos, string text, DrawMode drawMode = DrawMode.TopLeft) {
        if (drawMode is DrawMode.TopLeft or DrawMode.BottomLeft) { ParseLessDrawString(pos, text, drawMode); return;}
        ParseFullDrawString(pos, text, drawMode);
    }
    
    public enum DrawMode {
        TopLeft,              TopRight,
        
                    Center,
        
        BottomLeft,         BottomRight,
    }
}
