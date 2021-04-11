using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Infrastructure;
using MonoSnake.UI;

namespace MonoSnake.UI
{
    public class UiFrame
    {
        public int DrawOrder { get; }
        private Vector2 Position { get; }
        public float Rotation { get; set; }
        private Sprite _horizontalSprite;
        private Sprite _verticalSprite;
        private Sprite _topLeftSprite;
        private Sprite _topRightSprite;
        private Sprite _bottomLeftSprite;
        private Sprite _bottomRightSprite;
        private int _frameWidth;
        private int _frameHeight;
        private readonly List<UiObject> _topUiObjectRow = new List<UiObject>();
        private readonly List<UiObject> _rightUiObjectColumn = new List<UiObject>();
        private readonly List<UiObject> _bottomUiObjectRow = new List<UiObject>();
        private readonly List<UiObject> _leftUiObjectColumn = new List<UiObject>();
        private readonly UiObject _topLeftUiObject;
        private readonly UiObject _topRightUiObject;
        private readonly UiObject _bottomRightUiObject;
        private readonly UiObject _bottomLeftUiObject;
        private const float _rotate90CW = (float)(90 * Math.PI / 180);

        public UiFrame(Vector2 position, int width, int height, Sprite horizontalSprite, Sprite verticalSprite, Sprite topLeftSprite, Sprite topRightSprite, Sprite bottomRightSprite, Sprite bottomLeftSprite)
        {
            _frameWidth = width;
            _frameHeight = height;
            _horizontalSprite = horizontalSprite;
            _verticalSprite = verticalSprite;
            _topLeftSprite = topLeftSprite;
            _topRightSprite = topRightSprite;
            _bottomLeftSprite = bottomLeftSprite;
            _bottomRightSprite = bottomRightSprite;

            Position = position;

            _topLeftUiObject = new UiObject(_topLeftSprite, position, 0f);
            Vector2 nextTopRowPosition = position;

            nextTopRowPosition = new Vector2(nextTopRowPosition.X + _topLeftSprite.Width, nextTopRowPosition.Y);
            for (int i = 0; i < _frameWidth - 2; i++)
            {
                UiObject uiObject = new UiObject(_horizontalSprite, nextTopRowPosition, _rotate90CW);
                _topUiObjectRow.Add(uiObject);
                nextTopRowPosition = new Vector2(uiObject.Position.X + uiObject.Sprite.Width, uiObject.Position.Y);
            }

            _topRightUiObject = new UiObject(_topRightSprite, nextTopRowPosition, 0f);
            Vector2 nextRightColumnPosition = new Vector2(nextTopRowPosition.X, nextTopRowPosition.Y + _topRightSprite.Height);

            for (int i = 0; i < _frameHeight - 2; i++)
            {
                UiObject uiObject = new UiObject(_verticalSprite, nextRightColumnPosition, 0f);
                this._rightUiObjectColumn.Add(uiObject);
                nextRightColumnPosition = new Vector2(uiObject.Position.X, uiObject.Position.Y + _verticalSprite.Height);
            }

            _bottomRightUiObject = new UiObject(_bottomRightSprite, nextRightColumnPosition, 0f);
            Vector2 nextBottomRowPosition = new Vector2(nextRightColumnPosition.X - _bottomRightSprite.Width, nextRightColumnPosition.Y);

            for (int i = 0; i < _frameWidth - 2; i++)
            {
                UiObject uiObject = new UiObject(_horizontalSprite, nextBottomRowPosition, _rotate90CW);
                this._bottomUiObjectRow.Add(uiObject);
                nextBottomRowPosition = new Vector2(uiObject.Position.X - _horizontalSprite.Width, uiObject.Position.Y);
            }

            _bottomLeftUiObject = new UiObject(_bottomLeftSprite, nextBottomRowPosition, 0f);
            Vector2 nextLeftColumnPosition = new Vector2(nextBottomRowPosition.X, nextBottomRowPosition.Y - _bottomLeftSprite.Height);

            for (int i = 0; i < _frameHeight - 2; i++)
            {
                UiObject uiObject = new UiObject(_verticalSprite, nextLeftColumnPosition, 0f);
                this._leftUiObjectColumn.Add(uiObject);
                nextLeftColumnPosition = new Vector2(uiObject.Position.X, uiObject.Position.Y - _verticalSprite.Height);
            }
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _topLeftUiObject.Draw(spriteBatch, gameTime);
            foreach (UiObject uiObject in _topUiObjectRow)
            {
                uiObject.Draw(spriteBatch, gameTime);
            }
            _topRightUiObject.Draw(spriteBatch, gameTime);
            foreach (UiObject uiObject in _rightUiObjectColumn)
            {
                uiObject.Draw(spriteBatch, gameTime);
            }
            _bottomRightUiObject.Draw(spriteBatch, gameTime);
            foreach (UiObject uiObject in _bottomUiObjectRow)
            {
                uiObject.Draw(spriteBatch, gameTime);
            }
            _bottomLeftUiObject.Draw(spriteBatch, gameTime);
            foreach (UiObject uiObject in _leftUiObjectColumn)
            {
                uiObject.Draw(spriteBatch, gameTime);
            }
        }
    }
}
