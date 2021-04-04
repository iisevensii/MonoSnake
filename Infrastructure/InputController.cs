using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoSnake.GameObjects;
using System.Linq;

namespace MonoSnake.Infrastructure
{
    public class InputController
    {
        private readonly Snake _snake;

        public InputController(Snake snake)
        {
            _snake = snake;
        }

        public void ProcessInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            var player1GamePadState = GamePad.GetState(PlayerIndex.One);

            if(keyboardState.IsKeyDown(Keys.Space))
                Trace.WriteLine("Pause Breakpoint");

            if (keyboardState.IsKeyDown(Keys.Up) || player1GamePadState.IsButtonDown(Buttons.DPadUp))
                MoveSnakeHead(SnakeDirection.Up);
            else if (keyboardState.IsKeyDown(Keys.Down) || player1GamePadState.IsButtonDown(Buttons.DPadDown))
                MoveSnakeHead(SnakeDirection.Down);
            else if (keyboardState.IsKeyDown(Keys.Left) || player1GamePadState.IsButtonDown(Buttons.DPadLeft))
                MoveSnakeHead(SnakeDirection.Left);
            else if (keyboardState.IsKeyDown(Keys.Right) || player1GamePadState.IsButtonDown(Buttons.DPadRight))
                MoveSnakeHead(SnakeDirection.Right);
        }

        private void MoveSnakeHead(SnakeDirection direction)
        {
            _snake.SnakeHead.Direction = direction;
        }
    }
}
