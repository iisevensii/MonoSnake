using System;
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

        public event EventHandler ExitEvent;

        protected virtual void OnExit(EventArgs e)
        {
            EventHandler handler = this.ExitEvent;
            handler?.Invoke(this, e);
        }
        public void ProcessInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            var player1GamePadState = GamePad.GetState(PlayerIndex.One);

            if (player1GamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                OnExit(EventArgs.Empty);

            if (keyboardState.IsKeyDown(Keys.Space))
                Trace.WriteLine("Pause Breakpoint");

            if ((keyboardState.IsKeyDown(Keys.Up) || player1GamePadState.IsButtonDown(Buttons.DPadUp))
                && _snake.SnakeHead.Direction != SnakeDirection.Down)
                MoveSnakeHead(SnakeDirection.Up);
            else if ((keyboardState.IsKeyDown(Keys.Down) || player1GamePadState.IsButtonDown(Buttons.DPadDown))
                && _snake.SnakeHead.Direction != SnakeDirection.Up)
                MoveSnakeHead(SnakeDirection.Down);
            else if ((keyboardState.IsKeyDown(Keys.Left) || player1GamePadState.IsButtonDown(Buttons.DPadLeft))
                && _snake.SnakeHead.Direction != SnakeDirection.Right)
                MoveSnakeHead(SnakeDirection.Left);
            else if ((keyboardState.IsKeyDown(Keys.Right) || player1GamePadState.IsButtonDown(Buttons.DPadRight))
                && _snake.SnakeHead.Direction != SnakeDirection.Left)
                MoveSnakeHead(SnakeDirection.Right);
        }

        private void MoveSnakeHead(SnakeDirection direction)
        {
            _snake.SnakeHead.Direction = direction;
        }
    }
}
