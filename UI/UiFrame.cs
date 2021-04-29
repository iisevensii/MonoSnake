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
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        protected readonly Sprite _topLeftSprite;
        protected readonly Sprite _topRightSprite;
        protected readonly Sprite _bottomLeftSprite;
        protected readonly int _frameWidth;
        protected readonly int _frameHeight;

        private const float ROTATE90_CW = (float)(90 * Math.PI / 180);

        private readonly Sprite _bottomRightSprite;
        private readonly List<UiObject> _topUiObjectRow = new List<UiObject>();
        private readonly List<UiObject> _rightUiObjectColumn = new List<UiObject>();
        private readonly List<UiObject> _bottomUiObjectRow = new List<UiObject>();
        private readonly List<UiObject> _leftUiObjectColumn = new List<UiObject>();
        private readonly UiObject _topLeftUiObject;
        private readonly UiObject _topRightUiObject;
        private readonly UiObject _bottomRightUiObject;
        private readonly UiObject _bottomLeftUiObject;
        private readonly Color _backgroundColor;
        private readonly GraphicsDeviceManager _graphics;

        private Texture2D _backgroundTexture2D;
        private Rectangle _backgroundRectangle;
        protected readonly Sprite _topRowSprite;
        protected readonly Sprite _bottomRowSprite;
        protected readonly Sprite _leftColumnSprite;
        protected readonly Sprite _rightColumnSprite;

        public int ActualWidth { get; }
        public int ActualHeight { get; }
        
        public UiFrame(GraphicsDeviceManager graphics, Vector2 position, int width, int height, Sprite topRowSprite, Sprite bottomRowSprite, Sprite rightColumnSprite, Sprite leftColumnSprite, Sprite topLeftSprite, Sprite topRightSprite, Sprite bottomRightSprite, Sprite bottomLeftSprite, Color backgroundColor)
        {
            _graphics = graphics;
            _frameWidth = width;
            _frameHeight = height;
            _topRowSprite = topRowSprite;
            _bottomRowSprite = bottomRowSprite;
            _leftColumnSprite = leftColumnSprite;
            _rightColumnSprite = rightColumnSprite;
            _topLeftSprite = topLeftSprite;
            _topRightSprite = topRightSprite;
            _bottomLeftSprite = bottomLeftSprite;
            _bottomRightSprite = bottomRightSprite;
            _backgroundColor = backgroundColor;

            Position = position;

            _topLeftUiObject = new UiObject(_topLeftSprite, Position, 0f);
            _topRightUiObject = new UiObject(_topRightSprite, Position, 0f);
            _bottomRightUiObject = new UiObject(_bottomRightSprite, Position, 0f);
            _bottomLeftUiObject = new UiObject(_bottomLeftSprite, Position, 0f);

            ActualWidth += _topLeftSprite.Width;
            for (int i = 0; i < _frameWidth - 2; i++)
            {
                ActualWidth += _topRowSprite.Width;
            }
            ActualWidth += _topRightSprite.Width;

            ActualHeight += _topLeftSprite.Height;
            for (int i = 0; i < _frameHeight -2; i++)
            {
                ActualHeight += _leftColumnSprite.Height;
            }
            ActualHeight += _bottomLeftSprite.Height;
        }

        protected virtual void Initialize()
        {
            InitializeBorderObjects();

            CreateBackgroundRectangleAndTexture2D();
        }

        protected void CreateBackgroundRectangleAndTexture2D()
        {
            _backgroundRectangle = new Rectangle((int) Position.X, (int) Position.Y, ActualWidth - _topLeftSprite.Width, ActualHeight - _topLeftSprite.Height);

            _backgroundTexture2D = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            _backgroundTexture2D.SetData(new [] { Color.White });
        }

        protected void InitializeBorderObjects()
        {
            _topLeftUiObject.Position = Position;
            Vector2 nextTopRowPosition = Position;

            // Top Row
            nextTopRowPosition = new Vector2(nextTopRowPosition.X + _topLeftSprite.Width, nextTopRowPosition.Y);
            for (int i = 0; i < _frameWidth - 2; i++)
            {
                UiObject uiObject = new UiObject(_topRowSprite, nextTopRowPosition, 0f);
                _topUiObjectRow.Add(uiObject);
                nextTopRowPosition = new Vector2(uiObject.Position.X + uiObject.Sprite.Width, uiObject.Position.Y);
            }

            _topRightUiObject.Position = nextTopRowPosition;
            Vector2 nextRightColumnPosition = new Vector2(nextTopRowPosition.X, nextTopRowPosition.Y + _topRightSprite.Height);

            // Right Column
            for (int i = 0; i < _frameHeight - 2; i++)
            {
                UiObject uiObject = new UiObject(_rightColumnSprite, nextRightColumnPosition, 0f);
                this._rightUiObjectColumn.Add(uiObject);
                nextRightColumnPosition = new Vector2(uiObject.Position.X, uiObject.Position.Y + _rightColumnSprite.Height);
            }

            _bottomRightUiObject.Position = nextRightColumnPosition;
            Vector2 nextBottomRowPosition = new Vector2(nextRightColumnPosition.X - _bottomRightSprite.Width, nextRightColumnPosition.Y);

            // Bottom Row
            for (int i = 0; i < _frameWidth - 2; i++)
            {
                UiObject uiObject = new UiObject(_bottomRowSprite, nextBottomRowPosition, 0f);
                this._bottomUiObjectRow.Add(uiObject);
                nextBottomRowPosition = new Vector2(uiObject.Position.X - _bottomRowSprite.Width, uiObject.Position.Y);
            }

            _bottomLeftUiObject.Position = nextBottomRowPosition;
            Vector2 nextLeftColumnPosition = new Vector2(nextBottomRowPosition.X, nextBottomRowPosition.Y - _bottomLeftSprite.Height);

            // Left Column
            for (int i = 0; i < _frameHeight - 2; i++)
            {
                UiObject uiObject = new UiObject(_leftColumnSprite, nextLeftColumnPosition, 0f);
                this._leftUiObjectColumn.Add(uiObject);
                nextLeftColumnPosition = new Vector2(uiObject.Position.X, uiObject.Position.Y - _leftColumnSprite.Height);
            }
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_backgroundTexture2D, _backgroundRectangle, _backgroundColor);
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
