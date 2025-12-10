namespace TuiCommon;

public class TuiListButton(string text, Action onClick) : TuiListItem {
    private bool _selected;
    public override void Select() => _selected = true;
    public override void Activate() => onClick();
    public override void Deselect() => _selected = false;

    public override void Render((int x, int y) pos, int width) {
        if (List == null) return;
        if (_selected) {
            List.Sb.SetBackgroundColor(pos.x, pos.y, List.HighlightColor);
            List.Sb.SetForegroundColor(pos.x, pos.y, 0);
            List.Sb.ResetStyles(pos.x + width, pos.y);
        }
        else List.Sb.SetForegroundColor(pos.x, pos.y, List.HighlightColor);
        List?.Sb.DrawString(((int)(pos.x + width/2f), pos.y), _selected ? $"> {text} <": text, ScreenBase.DrawMode.Center);
    }
}