namespace TuiCli;

public class SshScreen : AnsiColorBase {
    private Action<object>? _callback;

    public void SetCallback(Action<object> callback) => _callback = callback;
    
    protected override void ShowError(object e) =>
        Console.Error.WriteLine(e);

    protected override void ClearScreen() => ClearBuffers();
    protected override void PushDisplay(object value) => _callback?.Invoke(value);
}