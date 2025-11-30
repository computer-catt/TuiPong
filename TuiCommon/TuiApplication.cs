namespace TuiPong;

public abstract class TuiApplication(ScreenBase screenBase) {
    protected ScreenBase Sb = screenBase;
    
    protected internal virtual void Start() {}
    protected internal abstract void Render();
    protected internal virtual void Update() {}
    
    protected internal virtual void OnKeyReceived(ConsoleKeyInfo key) {}
}