using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.GameObjects;

namespace MonoSnake.Infrastructure
{
    public class Apple : IGameObject
    {
        public int DrawOrder { get; }
        public Sprite Sprite { get; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public Apple(Sprite sprite, Vector2 position)
        {
            Sprite = sprite;
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, Position, Rotation);
        }
    }
}
