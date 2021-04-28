using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoSnake.Infrastructure
{
    public class TextEntry
    {
        private enum CycleDirection
        {
            Up,
            Down
        }

        private readonly Texture2D _cursorTexture2D;
        private readonly ISprite _letterEntrySpriteFrame0;
        private Vector2 _position;
        private int _entryPosition = 0;
        private float _currentCharBlinkTime = 0.375f;
        private double _blinkTimeElapsed = 0d;
        private bool _blinkOn = false;

        private Keys _currentChar = Keys.None;
        private string _inputString = String.Empty;
        private SpriteFont _font;

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public TextEntry(GraphicsDevice graphicsDevice, Vector2 position, SpriteFont font)
        {
            _position = position;
            _font = font;
            // Test High Score Entry Cursor (Static)
            if (_cursorTexture2D == null)
            {
                _cursorTexture2D = new Texture2D(graphicsDevice, 1, 1);
                _cursorTexture2D.SetData(new[] { Color.White });
            }

            _letterEntrySpriteFrame0 = new Sprite(_cursorTexture2D, 0, 0, 20, 5);
        }

        public void KeyInput(Keys key)
        {
            if (key == Keys.Up)
            {
                CycleLetter(key, CycleDirection.Up);
            }
            else if (key == Keys.Down)
            {
                CycleLetter(key, CycleDirection.Down);
            }
            else if (key == Keys.Space)
            {
                _inputString += " ";
                _entryPosition++;
            }
            else if (key == Keys.Back)
            {
                if (!string.IsNullOrWhiteSpace(_inputString))
                {
                    _inputString = _inputString.Substring(0, _inputString.Length - 1);
                }
            }
            if (key >= Keys.A && key <= Keys.Z)
            {
                // Input Char
                _inputString += key;
                _entryPosition++;
            }
        }

        private Keys CycleLetter(Keys key, CycleDirection cycleDirection)
        {
            if (cycleDirection == CycleDirection.Up)
            {
                if (_currentChar == Keys.None)
                {
                    _currentChar = Keys.A;
                }
                else if (_currentChar == Keys.Z)
                {
                    _currentChar = Keys.None;
                }
                else if (_currentChar >= Keys.A || _currentChar < Keys.Y)
                {
                    _currentChar++;
                }
            }
            else if (cycleDirection == CycleDirection.Down)
            {
                if (_currentChar == Keys.None)
                {
                    _currentChar = Keys.Z;
                }   
                else if (_currentChar == Keys.A)
                {
                    _currentChar = Keys.None;
                }
                else if (_currentChar >= Keys.A || _currentChar < Keys.Y)
                {
                    _currentChar--;
                }
            }

            return _currentChar;
        }

        public void Update(GameTime gameTime)
        {
            _blinkTimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;

            if (_blinkTimeElapsed >= _currentCharBlinkTime)
            {
                _blinkOn = !_blinkOn;
                _blinkTimeElapsed = 0d;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 stringScale = _font.MeasureString("A");
            int inputStringLength = string.IsNullOrWhiteSpace(_inputString) ? 0 : _inputString.Length;

            if (!string.IsNullOrWhiteSpace(_inputString))
            {
                spriteBatch.DrawString
                (
                    _font,
                    _inputString,
                    new Vector2(_position.X, _position.Y - stringScale.Y / 2),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }

            if (_currentChar == Keys.None)
            {
                stringScale = _font.MeasureString("A");
                if (_blinkOn)
                {
                    spriteBatch.Draw
                    (
                        _letterEntrySpriteFrame0.SpriteSheet,
                        new Vector2(_position.X + stringScale.X * inputStringLength, _position.Y + stringScale.Y / 2 - 10),
                        new Rectangle(_letterEntrySpriteFrame0.Left, _letterEntrySpriteFrame0.Top, _letterEntrySpriteFrame0.Width, _letterEntrySpriteFrame0.Height),
                        _letterEntrySpriteFrame0.TintColor,
                        0f,
                        _letterEntrySpriteFrame0.Origin,
                        _letterEntrySpriteFrame0.Scale,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
            else
            {
                stringScale = _font.MeasureString(_currentChar.ToString());
                if (_blinkOn)
                {
                    spriteBatch.DrawString
                    (
                        _font,
                        _currentChar.ToString(),
                        new Vector2(_position.X + stringScale.X * inputStringLength, _position.Y - stringScale.Y / 2),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }
    }
}
