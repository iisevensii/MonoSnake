using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Infrastructure;

namespace MonoSnake.UI
{
    public class UiObject : IUiObject
    {
        public int DrawOrder { get; }
        public Sprite Sprite { get; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public bool IsMouseOver { get; set; }

        public UiObject(Sprite sprite, Vector2 position, float rotation)
        {
            Sprite = sprite;
            Position = position;
            Rotation = rotation;
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
                Sprite.Draw(spriteBatch, Position);
        }
    }
}
