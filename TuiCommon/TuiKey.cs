namespace TuiCommon;

public class TuiKey {
    public readonly string Key;
    public readonly char KeyChar;
    
    public TuiKey(ConsoleKeyInfo consoleKeyInfo) {
        KeyChar = consoleKeyInfo.KeyChar;
        Key = consoleKeyInfo.Key.ToString();
    }

    public TuiKey(string key, char keyChar) {
        Key = key;
        KeyChar = keyChar;
    }
}