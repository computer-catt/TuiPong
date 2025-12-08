namespace TuiCommon;

public class TuiKey {
    public readonly string Key;
    public readonly string? Code;
    public readonly char? KeyChar;
    
    public TuiKey(ConsoleKeyInfo consoleKeyInfo) {
        KeyChar = consoleKeyInfo.KeyChar;
        Key = consoleKeyInfo.Key.ToString();
    }

    public TuiKey(string key, char character) {
        Key = key;
        KeyChar = character;
    }

    public TuiKey(string key, string code) {
        Key = key;
        Code = code;
        KeyChar = ParseWebKey(key);
    }

    char? ParseWebKey(string key) => key switch {
            "Space" => ' ',
            "Enter" => '\n',
            _ => key.Length == 1 ? key[0] : null
        };
}