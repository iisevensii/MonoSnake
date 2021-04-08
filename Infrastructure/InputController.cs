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
        public event EventHandler RestartEvent;

        protected virtual void OnExit(EventArgs e)
        {
            EventHandler handler = this.ExitEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnRestart(EventArgs e)
        {
            EventHandler handler = this.RestartEvent;
            handler?.Invoke(this, e);
        }

        private KeyboardState oldKeyboardState;
        private KeyboardState newKeyboardState;
        private GamePadState oldGamePadState;
        private GamePadState newGamePadState;

        /// <summary>
        /// Detects a single key press
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool WasKeyPressed(Keys key)
        {
            return oldKeyboardState.IsKeyDown(key) && newKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Detects a single button press
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        private bool WasButtonPressed(Buttons button)
        {
            return oldGamePadState.IsButtonDown(button) && newGamePadState.IsButtonUp(button);
        }

        /// <summary>
        /// Process Player Input
        /// </summary>
        public void ProcessInput()
        {
            newKeyboardState = Keyboard.GetState();
            newGamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();
            var player1GamePadState = GamePad.GetState(PlayerIndex.One);

            if (player1GamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                OnExit(EventArgs.Empty);

            if (WasKeyPressed(Keys.Enter) || WasButtonPressed(Buttons.Start))
                OnRestart(EventArgs.Empty);

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

            oldKeyboardState = newKeyboardState;
            oldGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        private void MoveSnakeHead(SnakeDirection direction)
        {
            _snake.SnakeHead.Direction = direction;
        }
    }
}
