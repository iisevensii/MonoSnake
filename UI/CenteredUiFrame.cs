using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoSnake.Infrastructure;

namespace MonoSnake.UI
{
    public class CenteredUiFrame : UiFrame
    {

        public new int ActualWidth { get; }
        public new int ActualHeight { get; }

        public int TopLeftSpriteWidth => _topLeftSprite.Width;
        public int TopLeftSpriteHeight => _topLeftSprite.Height;
        public int TopRightSpriteWidth => _topRightSprite.Width;
        public int TopRightSpriteHeight => _topRightSprite.Height;
        public int BottomLeftSpriteWidth => _bottomLeftSprite.Width;
        public int BottomLeftSpriteHeight => _bottomLeftSprite.Height;
        public int BottomRightSpriteWidth => _bottomRowSprite.Width;
        public int BottomRightSpriteHeight => _bottomRowSprite.Height;

        public CenteredUiFrame
            (
                GraphicsDeviceManager graphics,
                Vector2 position,
                int width,
                int height,
                int parentWidth,
                int parentHeight,
                Sprite topRowSprite,
                Sprite bottomRowSprite,
                Sprite leftColumnSprite,
                Sprite rightColumnSprite,
                Sprite topLeftSprite,
                Sprite topRightSprite,
                Sprite bottomRightSprite,
                Sprite bottomLeftSprite,
                Color backgroundColor
            )
            : base
                (
                    graphics,
                    position,
                    width,
                    height,
                    topRowSprite,
                    bottomRowSprite,
                    rightColumnSprite,
                    leftColumnSprite,
                    topLeftSprite,
                    topRightSprite,
                    bottomRightSprite,
                    bottomLeftSprite,
                    backgroundColor
                )
        {
            ActualWidth += _topLeftSprite.Width;
            for (int i = 0; i < _frameWidth - 2; i++)
            {
                ActualWidth += _topRowSprite.Width;
            }
            ActualWidth += _topRightSprite.Width;

            ActualHeight += _topLeftSprite.Height;
            for (int i = 0; i < _frameHeight - 2; i++)
            {
                ActualHeight += _leftColumnSprite.Height;
            }
            ActualHeight += _bottomLeftSprite.Height;

            Position = new Vector2(parentWidth / 2 - this.ActualWidth / 2 + _topRightSprite.Width /2, parentHeight / 2 - this.ActualHeight / 2 + _topRightSprite.Width /2);
            this.Initialize();
        }

        protected override void Initialize()
        {
            InitializeBorderObjects();

            CreateBackgroundRectangleAndTexture2D();
        }
    }
}
