using System.Numerics;

namespace TuiPong;

public class Pong : ScreenHandler {
    private FrameCounter? _frameCounter;
    
    Vector2 _ballPosition = new(-5, -5);
    Vector2 _ballVelocity = new(1, 0);

    private const int PaddleHeight = 8;
    private int _rPaddleY;
    private int _lPaddleY;

    protected override void Start() {
        RefreshSpeed = 16;
        _frameCounter = new();
        _frameCounter.StartCounter(2000);
    }

    private int ClampPaddleY(int position) => 
        Math.Clamp(position, (int)(-ScreenHeight / 2f + PaddleHeight / 2f + 1), (int)(ScreenHeight / 2f - PaddleHeight / 2f + 0.6f -1));
    
    protected override void Update() {
        _frameCounter!.PushNewFrame();
        DrawString((0,1), "TuiPong " + _frameCounter.GetFps(), DrawMode.Center);

        _rPaddleY = ClampPaddleY(_rPaddleY);
        _lPaddleY = ClampPaddleY(_lPaddleY);
        
        for (int i = 0; i < ScreenHeight; i+=3)
            ScreenText[i, ScreenWidth / 2] = '|';
        
        if (_ballPosition is { X: <= -3, Y: <= -3 }) 
            _ballPosition = new(ScreenWidth / 2f, ScreenHeight / 2f); // Runs once
        
        if (_ballPosition.Y > ScreenHeight - 2 || _ballPosition.Y < 1.5f) 
            _ballVelocity.Y = -_ballVelocity.Y; // Reflect from top and bottom with 1 line padding

        if (_ballPosition.Y < Center.y + PaddleHeight/2f + _lPaddleY && 
            _ballPosition.Y > Center.y - PaddleHeight/2f + _lPaddleY - 1&&
            _ballPosition.X < 3 && _ballPosition.X > 1) // Reflect off paddle
            _ballVelocity = Vector2.Normalize(new Vector2(6, -(_lPaddleY + Center.y - _ballPosition.Y)));
        
        
        if (_ballPosition.Y < Center.y + PaddleHeight/2f + _rPaddleY && 
            _ballPosition.Y > Center.y - PaddleHeight/2f + _rPaddleY - 1&&
            _ballPosition.X > ScreenWidth - 4 && _ballPosition.X < ScreenWidth) // Reflect off paddle
            _ballVelocity = Vector2.Normalize(new Vector2(-6, -(_rPaddleY + Center.y - _ballPosition.Y)));
        
        for (int i = -PaddleHeight/2; i < PaddleHeight/2; i++) 
            ScreenText[Center.y + i + _rPaddleY, ScreenWidth - 2] = '┃'; // Render paddle
        for (int i = -PaddleHeight/2; i < PaddleHeight/2; i++) 
            ScreenText[Center.y + i + _lPaddleY, 1] = '┃'; // Render paddle
        
        if (ScreenText.IsInBounds((int)_ballPosition.X, (int)_ballPosition.Y)) 
            ScreenText[(int)_ballPosition.Y, (int)_ballPosition.X] = '⬤'; // Draw the ball while in bounds 

        _ballPosition += _ballVelocity; // Apply physics

        if (_ballPosition.X > ScreenWidth + 5 || _ballPosition.X < -5) { // Game over overlay
            Console.ForegroundColor = ConsoleColor.Red;
            char[] letters = ['G','a','m','e',' ','o','v','e','r'];
            for (int i = 0; i < letters.Length; i++)
                ScreenText[ScreenHeight / 2, ScreenWidth/2 - letters.Length/2 + i] = letters[i];
        }

        for (int i = 0; i < ScreenWidth; i++) {
            ScreenText[0, i] = '─';
            ScreenText[ScreenHeight - 1, i] = '─';
        }
        
        if (_ballPosition.X < ScreenWidth + 30 && _ballPosition.X > -30) return;// Reset
        _ballPosition = new (-5f, -5f);
        Console.ResetColor();
    }

    public void RightPaddleUp() => _rPaddleY++;
    public void RightPaddleDown() => _rPaddleY--;
    
    public void LeftPaddleUp() => _lPaddleY++;
    public void LeftPaddleDown() => _lPaddleY--;


    protected override void OnKeyRecieved(ConsoleKeyInfo keyInfo) {
        ConsoleKey key = keyInfo.Key;
        switch (key) {
            case ConsoleKey.UpArrow:
                RightPaddleDown();
                break;
            case ConsoleKey.DownArrow:
                RightPaddleUp();
                break;
                
            case ConsoleKey.W:
                LeftPaddleDown();
                break;
            case ConsoleKey.S:
                LeftPaddleUp();
                break;
        }
    }
}