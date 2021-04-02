using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Infrastructure;

namespace MonoSnake.GameObjects
{
    public class SnakeHead : IGameObject
    {
        private const float MovementIncrement = 800f;
        public int DrawOrder => 0;
        public Sprite Sprite { get; }
        public Vector2 Position { get; set; }

        public SnakeDirection Direction { get; set; } = SnakeDirection.Right;
        public bool MovementPending { get; set; } = false;

        private int _framesSinceLastMovement = 0;

        public SnakeHead(Sprite sprite, Vector2 position)
        {
            Sprite = sprite;
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            // ToDo:
            _framesSinceLastMovement++;
            if (_framesSinceLastMovement < 20)
                return;
            float gameTimeSecondsElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float movementAmount = MovementIncrement * gameTimeSecondsElapsed;
            switch (Direction)
            {
                case SnakeDirection.Up:
                    Position = new Vector2(Position.X, Position.Y - movementAmount);
                    Sprite.Rotation = (float)(180 * Math.PI / 180);
                    break;
                case SnakeDirection.Right:
                    Position = new Vector2(Position.X + movementAmount, Position.Y);
                    Sprite.Rotation = (float)(270 * Math.PI / 180);
                    break;
                case SnakeDirection.Down:
                    Position = new Vector2(Position.X, Position.Y + movementAmount);
                    Sprite.Rotation = 0f;
                    break;
                case SnakeDirection.Left:
                    Position = new Vector2(Position.X - movementAmount, Position.Y);
                    Sprite.Rotation = (float)(90 * Math.PI / 180);
                    break;
            }

            _framesSinceLastMovement = 0;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, Position);
        }
    }
}
