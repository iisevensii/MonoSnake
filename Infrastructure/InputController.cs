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
