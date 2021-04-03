using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoSnake.GameObjects;

namespace MonoSnake.Infrastructure
{
    public class InputController
    {
        private readonly SnakeHead _snakeHead;

        public InputController(SnakeHead snakeHead)
        {
            _snakeHead = snakeHead;
        }

        public void ProcessInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            var player1GamePadState = GamePad.GetState(PlayerIndex.One);


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
            _snakeHead.Direction = direction;
        }
    }
}
