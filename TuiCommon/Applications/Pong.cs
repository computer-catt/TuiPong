using System.Numerics;

namespace TuiCommon.Applications;

public class Pong(ScreenBase screenBase) : TuiApplication(screenBase) {
    Vector2 _ballPosition = new(-5, -5);
    Vector2 _ballVelocity = new(1, 0);

    private const int PaddleHeight = 5;
    private int _rPaddleY;
    private int _lPaddleY;
    private int _lScore;
    private int _rScore;
    private bool _outOfBounds;
    
    protected internal override void Start() => Sb.UseFrameCounter();

    private int ClampPaddleY(int position) => 
        Math.Clamp(position, (int)(-Sb.ScreenHeight / 2f + PaddleHeight / 2f + 1), (int)(Sb.ScreenHeight / 2f - PaddleHeight / 2f + 0.6f -1));

    protected internal override void Tick() {
        if (_ballPosition is { X: <= -3, Y: <= -3 }) 
            _ballPosition = new(Sb.ScreenWidth / 2f, Sb.ScreenHeight / 2f); // Runs once
        
        if (_ballPosition.Y > Sb.ScreenHeight - 2 || _ballPosition.Y < 1.5f) 
            _ballVelocity.Y = -_ballVelocity.Y; // Reflect from top and bottom with 1 line padding

        if (_ballPosition.Y < Sb.Center.y + PaddleHeight/2f + _lPaddleY && 
            _ballPosition.Y > Sb.Center.y - PaddleHeight/2f + _lPaddleY - 1 &&
            _ballPosition.X < 3 && _ballPosition.X > 1) // Reflect off paddle
            _ballVelocity = Vector2.Normalize(new (6, -(_lPaddleY + Sb.Center.y - _ballPosition.Y)));
        
        
        if (_ballPosition.Y < Sb.Center.y + PaddleHeight/2f + _rPaddleY && 
            _ballPosition.Y > Sb.Center.y - PaddleHeight/2f + _rPaddleY - 1&&
            _ballPosition.X > Sb.ScreenWidth - 4 && _ballPosition.X < Sb.ScreenWidth) // Reflect off paddle
            _ballVelocity = Vector2.Normalize(new (-6, -(_rPaddleY + Sb.Center.y - _ballPosition.Y)));
        
        _ballPosition += _ballVelocity; // Apply physics

        _outOfBounds = _ballPosition.X > Sb.ScreenWidth + 5 || _ballPosition.X < -5;
        /*Console.ForegroundColor = ConsoleColor.Red;*/
        
        if (_ballPosition.X < Sb.ScreenWidth + 30 && _ballPosition.X > -30) return; // Reset
        if (_ballPosition.X < Sb.ScreenWidth + 30) {_lScore++; _ballVelocity.X = Math.Abs(_ballVelocity.X);}
        if (_ballPosition.X > -30) {_rScore++; _ballVelocity.X = -Math.Abs(_ballVelocity.X);}
        _ballPosition = new (-5f, -5f);
        /*Console.ResetColor();*/
    }

    protected internal override void Render() {
        Sb.DrawString((0,1), "TuiPong " + Sb.FrameCounter!.GetFps() + " " + Sb.FrameCounter!.GetFrames());
        
        for (int i = 0; i < Sb.ScreenHeight; i+=3)
            Sb.DrawChar(Sb.Center.x, i, '|');
        
        for (int i = -PaddleHeight/2; i < PaddleHeight/2; i++) 
            Sb.DrawChar(Sb.ScreenWidth - 2, Sb.Center.y + i + _rPaddleY, '┃'); // Render paddle
        for (int i = -PaddleHeight/2; i < PaddleHeight/2; i++) 
            Sb.DrawChar(1, Sb.Center.y + i + _lPaddleY, '┃'); // Render paddle

        if (Extensions.IsInBounds(Sb.ScreenHeight, Sb.ScreenWidth, (int)_ballPosition.X, (int)_ballPosition.Y))
            Sb.DrawChar((int)_ballPosition.X, (int)_ballPosition.Y, '⬤');/*'⬤');*/ // Draw the ball while in bounds 

        if (_outOfBounds) { // Game over overlay
            char[] letters = ['G','a','m','e',' ','o','v','e','r'];
            for (int i = 0; i < letters.Length; i++)
                Sb.DrawChar(Sb.Center.x - letters.Length/2 + i, Sb.Center.y, letters[i]);
        }

        for (int i = 0; i < Sb.ScreenWidth; i++) {
            Sb.DrawChar(i, 0, '─');
            Sb.DrawChar(i, Sb.ScreenHeight - 1, '─');
        }
        
        Sb.DrawString((Sb.Center.x -4, 2), _rScore.ToString(), ScreenBase.DrawMode.TopRight);
        Sb.DrawString((Sb.Center.x + 5, 2), _lScore.ToString());
    }

    private void RightPaddleUp() => _rPaddleY = ClampPaddleY(_rPaddleY+1);
    private void RightPaddleDown() => _rPaddleY = ClampPaddleY(_rPaddleY-1);
    
    private void LeftPaddleUp() => _lPaddleY = ClampPaddleY(_lPaddleY+1);
    private void LeftPaddleDown() => _lPaddleY = ClampPaddleY(_lPaddleY-1);


    protected internal override void OnKeyReceived(TuiKey key) {
        switch (key.Key) {
            case "ArrowUp":
            case "UpArrow":
                RightPaddleDown();
                break;
            case "ArrowDown":
            case "DownArrow":
                RightPaddleUp();
                break;
                
            case "w":
            case "W":
                LeftPaddleDown();
                break;
            case "s":
            case "S":
                LeftPaddleUp();
                break;
        }
    }
}