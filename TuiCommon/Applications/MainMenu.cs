namespace TuiCommon.Applications;

public class MainMenu(ScreenBase screenBase) : TuiApplication(screenBase) {
    private int _uptimeCount;
    private readonly int _introDurationTicks = 100;
    private byte? _colorByte = 232;

    private readonly TuiList _menuList = new(screenBase, 
        [
            new TuiListButton("Pong", () => { screenBase.SetApplication(new Pong(screenBase)); }),
            new TuiListButton("Wtest", () => { screenBase.SetApplication(new Wtest(screenBase)); }),
            new TuiListButton("Lotta Text Test", () => { screenBase.SetApplication(new LottaTextTest(screenBase)); }),
            new TuiListButton("Exit", screenBase.StopScreen),
        ],
        (100, 100),
        (50, 0));
    
    protected internal override void Tick() {
        _uptimeCount++;
        Sb.UsingColors = true;
        if (_uptimeCount <= _introDurationTicks) {
            float bgColor = 232f + _uptimeCount / (float)_introDurationTicks * 23f;
            _colorByte = (byte)bgColor;
            Sb.SetDirty();
        }
        else _colorByte = null;
    }

    private bool _MainMenuPositioned = false;
    protected internal override void Render() {
        if (!_MainMenuPositioned) {
            _MainMenuPositioned = true;
            OnResize();
        }
        if (_colorByte != null) Sb.SetForegroundColor(0,0, _colorByte.Value);
        int logoId = GetLogoId(Sb.ScreenWidth);/*Math.Clamp(Math.Min(Sb.ScreenHeight, Sb.ScreenWidth / 3) / 5, 0, LogoMipMaps.Length -1)*/;
        Sb.DrawString((Sb.Center.x, Sb.ScreenHeight /4), LogoMipMaps[logoId], ScreenBase.DrawMode.Center);
        if (_colorByte != null) _menuList.HighlightColor = _colorByte.Value;
        _menuList.Render();
        Sb.DrawString((0, Sb.ScreenHeight), "TuiHub 1.0.0/Chocolate", ScreenBase.DrawMode.BottomLeft);
        Sb.DrawString((Sb.ScreenWidth, Sb.ScreenHeight), "Copyleft Catburger Ltd. Do redistribute!", ScreenBase.DrawMode.BottomRight);
    }

    protected internal override void OnResize() {
        int width = Math.Clamp(Sb.ScreenWidth / 10, 10, 20);
        int height = Sb.ScreenHeight / 3;
        _menuList.SetSize((width, height));
        _menuList.SetPosition(((int)(Sb.Center.x - width/2f), (int)(Sb.Center.y + height/4f)));
        Sb.SetDirty();
    }

    protected internal override void OnKeyReceived(TuiKey key) {
        switch (key.Key) {
            case "ArrowUp":
            case "UpArrow":
                _menuList.UpItem();
                Sb.SetDirty();
                break;
            case "ArrowDown":
            case "DownArrow":
                _menuList.DownItem();
                Sb.SetDirty();
                break;
            case "Enter":
                Sb.SetDirty();
                _menuList.Activate();
                break;
        }
    }

    public byte GetLogoId(int width) {
        for (int i = LogoLengths.Length - 1; i >= 0; i--)
            if (LogoLengths[i] <=width)
                return (byte)i;
        return 0;
    }
    
    public readonly byte[] LogoLengths = [
        6,  // 0
        21, // 1
        23, // 2
        27, // 3
        32, // 4
        42, // 5
        83, // 6
    ];
    public readonly string[] LogoMipMaps = [
        // 0
        "TuiHub",
        // 1
        """
         ---    . /__/     / 
         / /_/ / /  / /_/ /_)
        """,
        // 2
        """
        ____     o  /  /       
         /  / / /  /--/  / / / 
        /  /_/ /  /  /  /_/ /_)
        """,
        // 3
        // This one's from https://www.asciiart.eu/text-to-ascii-art (slight slant)
        """ 
         ______     _ __ __     __ 
        /_  __/_ __(_) // /_ __/ / 
         / / / // / / _  / // / _ \
        /_/  \_,_/_/_//_/\_,_/_.__/
        """,
        // 4
        // So is this one, "slant" font, slightly edited
        """
          ______      _ __  __      __  
         /_  __/_  __(_) / / /_  __/ /  
          / / / / / / / /_/ / / / / /_  
         / / / /_/ / / __  / /_/ /  o \ 
        /_/  \__,_/_/_/ /_/\__,_/_.___/ 
        """,
        // 5
        """
          ___________         ____  ____          
         /          /     __ /   / /   /      ___ 
        /___    ___/_ ___(__)   /_/   /__ ___/   /
          /   /  /  //  /  /   __    /  //  /   / 
         /   /  /  //  /  /   / /   /  //  /  O  \
        /___/   \__,__/__/___/ /___/\__,__/__.___/
        """,
        // 6
        """
             _____________________                 ________    _______                     
            /                    /                /       /  /       /                     
           /_____        ______ /          ____  /       /  /       /              ______  
                 /      /    ____  ______ (____)/       /__/       /_____  ______ /      / 
                /      /   /    /  /    //    //                  //    /  /    //      /  
               /      /   /    /  /    //    //      ____        //    /  /    //      /   
              /      /   /    /__/    //    //       /  /       //    /__/    //    OO  \  
             /______/    \____,,_____//____//_______/  /________/\_____,,____//____..____/ 
        """];
}