namespace TuiCommon;

public abstract class ScreenBase {
    protected ScreenBase(string[] args) {
        // ReSharper disable StringLiteralTypo
        string error = "";
        foreach (string meowArg in args) {
            string arg = meowArg.ToLower().Trim('-');
            string[] splitArg = arg.Split(':');
            string flag = splitArg[0].Trim();
            string value = arg.Remove(0, flag.Length).TrimStart(':').Trim();
            if (!int.TryParse(value, out int intValue)) intValue = -1;
            
            Console.WriteLine($"{flag}     |     {value}");
            
            switch (flag) {
                case "manualscreenwrap":
                    ManualScreenwrap = true;
                    if (string.IsNullOrEmpty(value)) break;
                    ManualScreenwrap = !value.IsNo();
                    break;
                case "renderspeed":
                    if (intValue != -1) {
                        _renderSpeedOverride = true;
                        RenderSpeed = intValue;
                    }
                    else error += $"Cant parse {flag} integer\nProvided: {value}\n";
                    break;
                case "tickspeed":
                    if (intValue != -1) {
                        _tickSpeedOverride = true;
                        TickSpeed = intValue;
                    }
                    else error += $"Cant parse {flag} integer\nProvided: {value}\n";
                    break;
                case "framecountondraw":
                    _frameCountOnDraw = true;
                    if (string.IsNullOrEmpty(value)) break;
                    _frameCountOnDraw = !value.IsNo();
                    break;
                case "forcedisabledirtysystem":
                    _forceDisableDirtySystem = true;
                    if (string.IsNullOrEmpty(value)) break;
                    _forceDisableDirtySystem = !value.IsNo();
                    break;
                default:
                    error += $"{flag} is not a valid flag.\n";
                    break;
            }
        } 
        // ReSharper restore StringLiteralTypo

        if (!string.IsNullOrEmpty(error)) 
            // ReSharper disable once VirtualMemberCallInConstructor
            ShowError(error);
    }
    

    protected abstract void ShowError(object e);
    protected abstract void UpdateScreenBounds();
    protected abstract void PushDisplay(object value);
    
    private TuiApplication _application;
    
    public void SetApplication(TuiApplication application) {
        _application = application;
        _isUsingDirtySystem = false;
        _application.Start();
    }
    
    protected internal int ScreenWidth;
    protected internal int ScreenHeight;
    protected internal char[] ScreenText = new char[1];

    private readonly bool _renderSpeedOverride = false;
    protected internal int RenderSpeed { get; private set; } = 16; // 60 FPS ish
    protected internal void SetRenderSpeed(int speedMs) {
        if (!_renderSpeedOverride) RenderSpeed = speedMs; }
    
    private readonly bool _tickSpeedOverride = false;
    protected internal int TickSpeed { get; private set; } = 16; // 60 FPS ish
    protected internal void SetTickSpeed(int speedMs) {
        if (!_tickSpeedOverride) TickSpeed = speedMs; }
    
    protected internal bool ManualScreenwrap = false;
    protected internal (int y, int x) Center;
    
    protected bool Running;
    protected internal FrameCounter? FrameCounter { get; private set; }

    private bool _isUsingDirtySystem = false;

    private bool _isDirty = false;
    protected internal void SetDirty() => 
        _isDirty = _isUsingDirtySystem = !_forceDisableDirtySystem;

    private bool _frameCountOnDraw = false;

    private readonly bool _forceDisableDirtySystem = false;
    
    protected internal void UseFrameCounter(int intervalMs = 1000) {
        FrameCounter = null;
        FrameCounter = new(intervalMs);
    }
    
    public void StartScreen() {
        if (Running) return;
        Running = true;

        try {
            new Thread(RenderLoop).Start();
            new Thread(TickLoop).Start();
        }
        catch (PlatformNotSupportedException) {
            _ = AsyncRenderLoop();
            _ = AsyncTickLoop();
        }
    }

    private void TickLoop() {
        while (Running) {
            try {
                if (ScreenWidth > 0 && ScreenHeight > 0) _application.Tick();
                Thread.Sleep(TickSpeed);
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                Thread.Sleep(TickSpeed * 50);
            }
        }
    }
    
    private async Task AsyncTickLoop() {
        while (Running) {
            try {
                if (ScreenWidth > 0 && ScreenHeight > 0) _application.Tick();
                await Task.Delay(TickSpeed);
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                await Task.Delay(TickSpeed * 50);
            }
        }
    }
    
    private void RenderLoop() {
        while (Running) {
            try {
                if (!_frameCountOnDraw) FrameCounter?.PushNewFrame();
                if (!_isUsingDirtySystem || _isDirty) RenderIteration();
                _isDirty = false;
                Thread.Sleep(RenderSpeed);
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                Thread.Sleep(RenderSpeed * 50);
            }
        }
    }
    private async Task AsyncRenderLoop() {
        while (Running) {
            try {
                if (!_frameCountOnDraw) FrameCounter?.PushNewFrame();
                if (!_isUsingDirtySystem || _isDirty) RenderIteration();
                _isDirty = false;
                await Task.Delay(RenderSpeed);
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                await Task.Delay(RenderSpeed * 50);
            }
        }
    }

    protected virtual void RenderIteration() {
        if (_frameCountOnDraw) FrameCounter?.PushNewFrame();
        UpdateScreenBounds();
        _application.Render();
        PushDisplay(ManualScreenwrap ? 
            ScreenText.ToStringBuilder(ScreenWidth, ScreenHeight) : 
            ScreenText);
    }
    
    public void StopScreen() => Running = false;

    private string _currentInput;
    private Action<string>? _currentTextCallback;
    private Action<string> _finalString;

    public void SendKey(TuiKey key) {
        if (key.Key == "Escape") {
            StopScreen();
            return;
        }
        
        if (ApplyUserInput(key)) return;
        
        _application.OnKeyReceived(key);
    }

    /// <returns>true if you should return early to avoid key passthrough.</returns>
    private bool ApplyUserInput(TuiKey key) {
        if (_currentTextCallback == null) return false;
        if (key.Key == "Backspace") {
            if (_currentInput.Length != 0) 
                _currentTextCallback(_currentInput = _currentInput[..^1]);
            return true;
        }
        
        if (key.Key != "Enter") {
            _currentTextCallback(_currentInput += key.KeyChar);
            return true;
        }
        
        _finalString(_currentInput);
        _currentTextCallback = null;
        _currentInput = "";
        return true;
    }
    
    protected internal void GetUserInput(Action<string> currentTextCallback, Action<string> finalString) {
        _currentTextCallback = currentTextCallback;
        _finalString = finalString;
    }
    
    public int ResolveCharPos(int x, int y) => y * ScreenWidth + x;
    protected internal void DrawChar(int x, int y, char ch) => ScreenText[ResolveCharPos(x,y)] = ch;
    
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