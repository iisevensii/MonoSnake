using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSnake.Infrastructure
{
    public class Sprite : ISprite
    {
        private Rectangle _rectangle;
        public Texture2D SpriteSheet { get; }
        public int Top { get; }
        public int Left { get; }
        public int Width { get; }
        public int Height { get; }
        public Color TintColor { get; set; } = Color.White;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public Vector2 Scale { get; set; } = Vector2.One;

        public Sprite(Texture2D spriteSheet, int top, int left, int width, int height)
        {
            SpriteSheet = spriteSheet ?? throw new ArgumentException("Texture2D must not be null", nameof(spriteSheet));
            Top = top;
            Left = left;
            Width = width;
            Height = height;

            _rectangle = new Rectangle(Left, Top, Width, Height);
        }

        public Sprite(PositionedTexture2D positionedTexture2D, int width, int height)
        {
            SpriteSheet = positionedTexture2D.SpriteSheet ?? throw new ArgumentException("PositionedTexture2D must not be null", nameof(positionedTexture2D)); ;
            Width = width;
            Height = height;
            int margin = positionedTexture2D.Margin;
            int xIndex = positionedTexture2D.SprintXIndex;
            int yIndex = positionedTexture2D.SpriteYIndex;

            Left = (xIndex * (margin * 2)) + margin + (width * xIndex);
            Top = (yIndex * (margin * 2)) + margin + (width * yIndex);

            _rectangle = new Rectangle(Left, Top, Width, Height);
        }

        public void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            spriteBatch.Draw
                (
                    SpriteSheet,
                    position,
                    _rectangle,
                    TintColor,
                    rotation,
                    Origin,
                    Scale,
                    SpriteEffects.None,
                    0f
                );
        }
    }
}
