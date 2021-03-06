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

        private const int INPUT_STRING_MAX_LENGTH = 15;

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

        public string InputtedString => _currentChar != Keys.None ? $"{_inputString}{_currentChar}" : _inputString;

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
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

        public void Reset()
        {
            _currentChar = Keys.None;
            _inputString = string.Empty;
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
                if (_currentChar != Keys.None)
                {
                    TryAddCharacterToInputString(InputKeyToCharString(_currentChar));
                    _entryPosition = _inputString.Length;
                    _currentChar = Keys.None;
                }
            }
            else if (key == Keys.Back)
            {
                if (!string.IsNullOrWhiteSpace(_inputString))
                {
                    _inputString = _inputString.Substring(0, _inputString.Length - 1);
                    _currentChar = Keys.None;
                }
            }
            // Input Char
            else if (key.IsAlphabetKey() || key.IsTopRowNumberKey() || key.IsNumPadNumberKey())
            {
                TryAddCharacterToInputString(InputKeyToCharString(key));

                _entryPosition = _inputString.Length;
                _currentChar = Keys.None;
            }
        }

        private char InputKeyToCharString(Keys key)
        {
            char result = '_';

            if (key.IsAlphabetKey())
                return char.Parse(key.ToString());

            switch (key)
            {
                case Keys.D0:
                case Keys.NumPad0:
                    result = '0';
                    break;
                case Keys.D1:
                case Keys.NumPad1:
                     result = '1';
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                     result = '2';
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                     result = '3';
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                     result = '4';
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                     result = '5';
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                     result = '6';
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                     result = '7';
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                     result = '8';
                    break;
                case Keys.D9:
                case Keys.NumPad9:
                     result = '9';
                    break;
            }

            return result;
        }

        private Keys CycleLetter(Keys key, CycleDirection cycleDirection)
        {
            if (_inputString.Length == INPUT_STRING_MAX_LENGTH)
                return Keys.None;

            if (cycleDirection == CycleDirection.Up)
            {
                if (_currentChar == Keys.None)
                {
                    _currentChar = Keys.A;
                }
                else if (_currentChar == Keys.D9)
                {
                    _currentChar = Keys.None;
                }
                else if (_currentChar.IsAlphabetKey() || _currentChar.IsTopRowNumberKey())
                {
                    if(_currentChar < Keys.Z)
                        _currentChar++;
                    else if (_currentChar == Keys.Z)
                        _currentChar = Keys.D0;
                    else if (_currentChar.IsTopRowNumberKey())
                        _currentChar++;
                }
            }
            else if (cycleDirection == CycleDirection.Down)
            {
                if (_currentChar == Keys.None)
                {
                    _currentChar = Keys.D9;
                }
                else if (_currentChar == Keys.D0)
                {
                    _currentChar = Keys.Z;
                }
                else if (_currentChar == Keys.A)
                {
                    _currentChar = Keys.None;
                }
                else if (_currentChar.IsAlphabetKey() || _currentChar.IsTopRowNumberKey())
                {
                    if (_currentChar.IsAlphabetKey())
                        _currentChar--;
                    else if (_currentChar == Keys.None)
                        _currentChar = Keys.D9;
                    else if (_currentChar.IsTopRowNumberKey())
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
                    if(inputStringLength < INPUT_STRING_MAX_LENGTH)
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
                stringScale = _currentChar.IsTopRowNumberKey()
                    ? _font.MeasureString(InputKeyToCharString(_currentChar).ToString())
                    : _font.MeasureString(_currentChar.ToString());

                if (_blinkOn)
                {
                    spriteBatch.DrawString
                    (
                        _font,
                        InputKeyToCharString(_currentChar).ToString(),
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

        private void TryAddCharacterToInputString(char character)
        {
            if (_inputString.Length < INPUT_STRING_MAX_LENGTH)
            {
                _inputString += character;
            }
        }
    }
}
