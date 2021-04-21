using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoSnake.Infrastructure;

namespace MonoSnake.UI
{
    public class CenteredUiFrame : UiFrame
    {
        public CenteredUiFrame
            (
                GraphicsDeviceManager graphics,
                Vector2 position,
                int width,
                int height,
                int parentWidth,
                int parentHeight,
                Sprite horizontalSprite,
                Sprite verticalSprite,
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
                    horizontalSprite,
                    verticalSprite,
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
                ActualWidth += _horizontalSprite.Width;
            }
            ActualWidth += _topRightSprite.Width;

            ActualHeight += _topLeftSprite.Height;
            for (int i = 0; i < _frameHeight - 2; i++)
            {
                ActualHeight += _verticalSprite.Height;
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
