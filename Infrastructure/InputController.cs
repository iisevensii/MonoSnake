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

        public event EventHandler ExitEvent;
        public event EventHandler RestartEvent;
        public event EventHandler StartEvent;
        public event EventHandler HeadTurnEvent;

        private KeyboardState _oldKeyboardState;
        private KeyboardState _newKeyboardState;
        private GamePadState _oldGamePadState;
        private GamePadState _newGamePadState;
        private TextEntry _textEntry;
        private SnakeGame _snakeGame;

        public UIState UIState { get; set; } = UIState.GamePlay;

        public InputController(SnakeGame snakeGame, Snake snake, TextEntry textEntry)
        {
            _snakeGame = snakeGame;
            _snake = snake;
            _textEntry = textEntry;
            _snakeGame.UIStateChangeEvent += _snakeGame_UIStateChangeEvent;
        }

        private void _snakeGame_UIStateChangeEvent(SnakeGame snakeGame, UIState uiState)
        {
            this.UIState = uiState;
        }

        protected virtual void OnStart(EventArgs e)
        {
            EventHandler handler = this.StartEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnRestart(EventArgs e)
        {
            EventHandler handler = this.RestartEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnExit(EventArgs e)
        {
            EventHandler handler = this.ExitEvent;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Detects a single key press
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool WasKeyPressed(Keys key)
        {
            return _oldKeyboardState.IsKeyDown(key) && _newKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Detects a single button press
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        private bool WasButtonPressed(Buttons button)
        {
            return _oldGamePadState.IsButtonDown(button) && _newGamePadState.IsButtonUp(button);
        }

        /// <summary>
        /// Process Player Input
        /// </summary>
        public void ProcessInput()
        {
            _newKeyboardState = Keyboard.GetState();
            _newGamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();
            var player1GamePadState = GamePad.GetState(PlayerIndex.One);

            // ToDo: Remove this code
            if (UIState != UIState.GamePlay && WasKeyPressed(Keys.F1))
            {
                UIState = UIState.HighScoreEntry;
            }

            if (UIState == UIState.HighScoreEntry)
            {
                if (WasKeyPressed(Keys.Up) || WasButtonPressed(Buttons.DPadUp))
                {
                    _textEntry.KeyInput(Keys.Up);
                }
                else if (WasKeyPressed(Keys.Down) || WasButtonPressed(Buttons.DPadDown))
                {
                    _textEntry.KeyInput(Keys.Down);
                }
            }

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

            _oldKeyboardState = _newKeyboardState;
            _oldGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        private void MoveSnakeHead(SnakeDirection direction)
        {
            bool isDifferent = direction != _snake.LastInputDirection;
            _snake.LastInputDirection = direction;
            if (isDifferent)
                HeadTurnEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
