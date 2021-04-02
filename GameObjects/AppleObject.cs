using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Infrastructure;

namespace MonoSnake.GameObjects
{
    public class AppleObject : IGameObject
    {
        public int DrawOrder => 0;
        public Sprite Sprite { get; }
        public Vector2 Position { get; set; }

        public AppleObject(Sprite sprite, Vector2 position)
        {
            Sprite = sprite;
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            // ToDo:
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, Position);
        }
    }
}
