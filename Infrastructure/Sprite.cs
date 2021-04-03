using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSnake.Infrastructure
{
    public class Sprite
    {
        private Texture2D SpriteSheet { get; }
        private int Top { get; }
        private int Left { get; }
        private int Width { get; }
        private int Height { get; }
        public Color TintColor { get; set; } = Color.White;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public Vector2 Scale { get; set; } = Vector2.One;

        public Sprite(Texture2D spriteSheet, int top, int left, int width, int height)
        {
            SpriteSheet = spriteSheet;
            Top = top;
            Left = left;
            Width = width;
            Height = height;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            spriteBatch.Draw
                (
                    SpriteSheet,
                    position,
                    new Rectangle(Left, Top, Width, Height),
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
