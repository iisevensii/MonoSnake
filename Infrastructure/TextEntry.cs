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

        private readonly Texture2D _letterEntryTexture2DFrame0;
        private readonly Texture2D _letterEntryTexture2DFrame1;
        private readonly ISprite _letterEntrySpriteFrame0;
        private readonly ISprite _letterEntrySpriteFrame1;
        private readonly ISprite _letterEntryAnimatedSprite;
        private Vector2 _position;
        private int _entryPosition = 0;

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
            if (_letterEntryTexture2DFrame0 == null)
            {
                _letterEntryTexture2DFrame0 = new Texture2D(graphicsDevice, 1, 1);
                _letterEntryTexture2DFrame0.SetData(new[] { Color.White });
            }

            if (_letterEntryTexture2DFrame1 == null)
            {
                _letterEntryTexture2DFrame1 = new Texture2D(graphicsDevice, 1, 1);
                _letterEntryTexture2DFrame1.SetData(new[] { Color.Transparent });
            }

            _letterEntrySpriteFrame0 = new Sprite(_letterEntryTexture2DFrame0, 0, 0, 20, 5);
            _letterEntrySpriteFrame1 = new Sprite(_letterEntryTexture2DFrame1, 0, 0, 20, 5);

            List<AnimatedSprite.AnimatedSpriteFrame> letterEntryAnimatedSpriteFrames =
                new List<AnimatedSprite.AnimatedSpriteFrame>
                {
                    new AnimatedSprite.AnimatedSpriteFrame(_letterEntrySpriteFrame0, 0.5f),
                    new AnimatedSprite.AnimatedSpriteFrame(_letterEntrySpriteFrame1, 0.5f)
                };
            _letterEntryAnimatedSprite = new AnimatedSprite(letterEntryAnimatedSpriteFrames, 0, 0, 0, 0);
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

            if (key >= Keys.A && key <= Keys.Z)
            {
                // Input Char
                _inputString = _inputString + key;
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
            _letterEntryAnimatedSprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            _letterEntryAnimatedSprite.Draw(spriteBatch, _position, 0f);
        }
    }
}
