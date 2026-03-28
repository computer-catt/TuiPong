using System.Text;
using TuiCommon.Applications;

namespace TuiCommon;

public abstract class ScreenBase {
    private TuiAppSettings _defaultSettings = new();
    public void SetArgs(string[] args) {
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
                    if (intValue == -1) {
                        error += $"Cant parse {flag} integer\nProvided: {value}\n";
                        break;
                    }
                    _renderSpeedOverride = true;
                    _defaultSettings.RenderSpeed = intValue;
                    break;
                case "tickspeed":
                    if (intValue == -1) {
                        error += $"Cant parse {flag} integer\nProvided: {value}\n";
                        break;
                    }
                    _tickSpeedOverride = true;
                    _defaultSettings.TickSpeed = intValue;
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
                case "disablecolors":
                    _disableColors = true;
                    if (string.IsNullOrEmpty(value)) break;
                    _disableColors = !value.IsNo();
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
    protected abstract void ClearScreen();
    protected abstract void PushDisplay(object value);
    
    private TuiApplication? _application;
    public TuiAppSettings Settings /*{ get; private set; }*/ = new();
    
    public void SetApplication(TuiApplication application) {
        _application = application;
        Settings = _defaultSettings;
        _application.Start();
    }
    
    protected internal int ScreenWidth;
    protected internal int ScreenHeight;
    
    public virtual void SetScreenBounds(int width, int height, bool colors = true) {
        ScreenHeight = height;
        ScreenWidth = width;
        int size = ScreenHeight * ScreenWidth;
        ScreenText        = new char [size];
        if (colors) {
            BackgroundColors  = new byte?[size];
            ForegroundColors  = new byte?[size];
            RefreshCharBuffer = new bool [size];    
        }
        
        Center = (ScreenWidth / 2, ScreenHeight / 2);
        SetDirtyOptional();
        _application?.OnResize();
    }
    
    protected internal char[] ScreenText = new char[1];
    private bool _disableColors = false;
    protected internal byte?[] ForegroundColors = new byte?[1];
    protected internal byte?[] BackgroundColors = new byte?[1];
    protected internal bool[] RefreshCharBuffer = new bool[1];

    protected void ClearBuffers(bool fill = false) {
        if (fill) Array.Fill(ScreenText,' ');
        else Array.Clear(ScreenText);
        if (!Settings.UsingColors) return;
        Array.Clear(ForegroundColors);
        Array.Clear(BackgroundColors);
        Array.Clear(RefreshCharBuffer);
    }
    
    private bool _renderSpeedOverride = false;
    protected internal void SetRenderSpeed(int speedMs) {
        if (!_renderSpeedOverride) Settings.RenderSpeed = speedMs; }
    
    private bool _tickSpeedOverride = false;
    protected internal void SetTickSpeed(int speedMs) {
        if (!_tickSpeedOverride) Settings.TickSpeed = speedMs; }
    
    protected internal bool ManualScreenwrap = false;
    protected internal (int x, int y) Center;
    
    protected bool Running;
    protected internal FrameCounter? FrameCounter { get; private set; }

    private bool _isDirty = false;
    protected internal void SetDirty() => 
        _isDirty = Settings.UsingDirtySystem = !_forceDisableDirtySystem;

    protected internal void SetDirtyOptional() => _isDirty = true;

    private bool _frameCountOnDraw = false;

    private bool _forceDisableDirtySystem = false;
    
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
                if (ScreenWidth > 0 && ScreenHeight > 0) _application?.Tick();
                Thread.Sleep(Settings.TickSpeed);
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                Thread.Sleep(Settings.TickSpeed * 50);
            }
        }
    }
    
    private async Task AsyncTickLoop() {
        while (Running) {
            try {
                if (ScreenWidth > 0 && ScreenHeight > 0) _application?.Tick();
                await Task.Delay(Settings.TickSpeed);
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                await Task.Delay(Settings.TickSpeed * 50);
            }
        }
    }
    
    private void RenderLoop() {
        while (Running) {
            try {
                if (!_frameCountOnDraw) FrameCounter?.PushNewFrame();
                if (!Settings.UsingDirtySystem || _isDirty) RenderIteration();
                _isDirty = false;
                Thread.Sleep(Settings.RenderSpeed);
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                Thread.Sleep(Settings.RenderSpeed * 50);
            }
        }
    }
    private async Task AsyncRenderLoop() {
        while (Running) {
            try {
                if (!_frameCountOnDraw) FrameCounter?.PushNewFrame();
                if (!Settings.UsingDirtySystem || _isDirty) RenderIteration();
                _isDirty = false;
                await Task.Delay(Settings.RenderSpeed);
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                await Task.Delay(Settings.RenderSpeed * 50);
            }
        }
    }

    protected virtual StringBuilder BuildManualWrapString() =>
        ScreenText.ToStringBuilder(ScreenWidth, ScreenHeight);

    protected virtual object BuildAutoWrapString() => ScreenText;
    
    protected virtual void RenderIteration() {
        if (_frameCountOnDraw) FrameCounter?.PushNewFrame();
        ClearScreen();
        _application?.Render();
        PushDisplay(ManualScreenwrap ? 
            BuildManualWrapString(): 
            BuildAutoWrapString());
    }

    public event Action? Stopping;
    public void StopScreen() {
        Stopping?.Invoke();
        Running = false;
    }

    private string _currentInput = "";
    private Action<string>? _currentTextCallback;
    private Action<string>? _finalString;

    public void SendKey(TuiKey key) {
        if (key.Key == "Escape") {
            SetApplication(new MainMenu(this));
            return;
        }
        
        if (ApplyUserInput(key)) return;
        
        _application?.OnKeyReceived(key);
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
        
        _finalString!(_currentInput);
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

    protected internal void SetBackgroundColor(int x, int y, byte color) {
        if (_disableColors) return;if (_disableColors) return;
        Settings.UsingColors = true;
        BackgroundColors[ResolveCharPos(x,y)] = color;
    }

    protected internal void SetForegroundColor(int x, int y, byte color) {
        if (_disableColors) return;
        Settings.UsingColors = true;
        ForegroundColors[ResolveCharPos(x,y)] = color;
    }

    protected internal void ResetStyles(int x, int y) {
        if (_disableColors) return;
        Settings.UsingColors = true;
        RefreshCharBuffer[ResolveCharPos(x,y)] = true;
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
            if (text[charI] == '\n') { line++; charPos = 0; continue;}

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