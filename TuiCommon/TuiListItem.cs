namespace TuiCommon;

public abstract class TuiListItem {
    protected TuiList? List;
    public void SetContext(TuiList context) => List = context;
    public abstract void Select();
    public abstract void Activate();
    public abstract void Deselect();
    public abstract void Render((int x, int y) pos, int width);
}