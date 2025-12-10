namespace TuiCommon;

public class TuiList {
    public TuiList(ScreenBase sb, TuiListItem[] listItems, (int width, int height) size, (int x, int y) position) {
        Sb = sb;
        SetItems(listItems);
        _size = size;
        _position = position;
        SetContexts();
        SelectedItem.Select();
    }
    
    public ScreenBase Sb;
    public void SetItems(TuiListItem[] items) {
        _listItems = items.ToList();
        _itemCount = _listItems.Count;
    }
    private List<TuiListItem> _listItems;
    public void SetSize((int width, int height) size) => _size = size;
    private (int width, int height) _size;
    public void SetPosition((int x, int y) position) => _position = position;
    private (int x, int y) _position;

    private int _itemCount;

    public byte HighlightColor = 255;

    public void SetContexts() {
        foreach (var item in _listItems)
            item.SetContext(this);
    }

    public void AddItem(TuiListItem item) {
        _listItems.Add(item);
        _itemCount++;
    }

    public void RemoveItem(int index) => _listItems.RemoveAt(index);

    private int _selectedIndex;

    private TuiListItem SelectedItem {
        get {
            if (_listItems.Count < _selectedIndex) _selectedIndex = _listItems.Count - 1;
            return _listItems[_selectedIndex];
        }

        set {
            int index = _listItems.IndexOf(value);
            if (index != -1) _selectedIndex = index;
        }
    }

    public void UpItem() {
        SelectedItem.Deselect();
        if (_selectedIndex != 0) 
            _selectedIndex--;
        SelectedItem.Select();
    }

    public void DownItem() {
        SelectedItem.Deselect();
        if (_selectedIndex != _itemCount -1) 
            _selectedIndex++;
        SelectedItem.Select();
    }

    public void Activate() => SelectedItem.Activate();
    
    public void Render() {
        for (var index = 0; index < _listItems.Count; index++) {
            TuiListItem item = _listItems[index];
            item.Render((_position.x, _position.y + index), _size.width);
        }
    }
}