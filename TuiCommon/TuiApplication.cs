namespace TuiCommon;

public abstract class TuiApplication(ScreenBase screenBase) {
    protected ScreenBase Sb = screenBase;
    
    protected internal virtual void Start() {}
    protected internal virtual void OnResize() {}
    protected internal abstract void Render();
    protected internal virtual void Tick() {}
    
    protected internal virtual void OnKeyReceived(TuiKey key) {}
}