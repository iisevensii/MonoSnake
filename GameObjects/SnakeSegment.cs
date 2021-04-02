using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Infrastructure;

namespace MonoSnake.GameObjects
{
    public class SnakeSegment : IGameObject
    {

        public int DrawOrder => 0;
        public Sprite Sprite { get; }
        public Vector2 Position { get; set; }
        public Vector2 PreviousSnakeSegmentPosition { get; set; }

        public SnakeSegment(Sprite sprite, Vector2 position)
        {
            Sprite = sprite;
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            Position = PreviousSnakeSegmentPosition;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, Position);
        }
    }
}
