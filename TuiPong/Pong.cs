using System.Numerics;

namespace TuiPong;

public class Pong : ScreenHandler {
    Vector2 _ballPosition = new(-5, -5);
    Vector2 _ballVelocity = new(.5f, .0f);

    private int paddleHeight = 8;
    private int paddleY;

    protected override void Start() => RefreshSpeed = 16;

    private char DisplaySpot;
    
    protected override void Update() {
        ScreenText[5, 5] = DisplaySpot;
        
        paddleY = Math.Clamp(paddleY, (int)(-ScreenHeight/2f + paddleHeight / 2f), (int)(ScreenHeight / 2f - paddleHeight / 2f + 0.6f));
        
        for (int i = 0; i < ScreenHeight; i+=3)
            ScreenText[i, ScreenWidth / 2] = '|';
        
        if (_ballPosition is { X: <= -3, Y: <= -3 }) 
            _ballPosition = new(ScreenWidth / 2f, ScreenHeight / 2f); // Runs once
        
        if (_ballPosition.Y > ScreenHeight - 1 || _ballPosition.Y < 0.5f) 
            _ballVelocity.Y = -_ballVelocity.Y; // Reflect from top and bottom with 1 line padding

        if (_ballPosition.X < 0) _ballVelocity.X = -_ballVelocity.X; // Reflect off the left

        if (_ballPosition.Y < ScreenHeight/2f + paddleHeight/2f + paddleY && 
            _ballPosition.Y > ScreenHeight/2f - paddleHeight/2f + paddleY &&
            _ballPosition.X > ScreenWidth - 2 && _ballPosition.X < ScreenWidth) // Reflect off paddle
            _ballVelocity = Vector2.Normalize(new Vector2(-6, -(paddleY + ScreenHeight/2f - _ballPosition.Y))/2);
        
        for (int i = -paddleHeight/2; i < paddleHeight/2; i++) 
            ScreenText[ScreenHeight/2 + i + paddleY, ScreenWidth - 2] = '┃'; // Render paddle
        
        if (ScreenText.IsInBounds((int)_ballPosition.X, (int)_ballPosition.Y)) 
            ScreenText[(int)_ballPosition.Y, (int)_ballPosition.X] = '⬤'; // Draw the ball while in bounds 

        _ballPosition += _ballVelocity; // Apply physics

        if (_ballPosition.X > ScreenWidth + 5) { // Game over overlay
            Console.ForegroundColor = ConsoleColor.Red;
            char[] letters = ['G','a','m','e',' ','o','v','e','r'];
            for (int i = 0; i < letters.Length; i++)
                ScreenText[ScreenHeight / 2, ScreenWidth/2 - letters.Length/2 + i] = letters[i];
        }

        if (_ballPosition.X < ScreenWidth + 30) return;// Reset
        _ballPosition = new (-5f, -5f);
        Console.ResetColor();
    }

    public void PaddleUp() => paddleY++;
    public void PaddleDown() => paddleY--;
}