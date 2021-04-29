using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSnake.Infrastructure
{
    public interface ISprite
    {
        Texture2D SpriteSheet { get; }
        int Top { get; }
        int Left { get; }
        int Width { get; }
        int Height { get; }
        public Color TintColor { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }
        float Rotation { get; set; }
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, Vector2 position);
    }
}
