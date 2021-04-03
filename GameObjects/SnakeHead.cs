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
        private const float MovementIncrement = 1000f;
        public int DrawOrder => 0;
        public Sprite Sprite { get; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public SnakeDirection Direction { get; set; } = SnakeDirection.Right;
        public bool MovementPending { get; set; } = false;

        private int _framesSinceLastMovement;

        public SnakeHead(Sprite sprite, Vector2 position)
        {
            Sprite = sprite;
            Position = position;
        }

        public bool CanUpdate()
        {
            // Update Frame Counter
            _framesSinceLastMovement++;

            // Require at least 20 frames to have passed before moving.
            // Direction from InputController is already set so it is not being lost
            if (_framesSinceLastMovement < 20)
                return false;

            return true;
        }

        public void Update(GameTime gameTime)
        {
            if (!CanUpdate())
                return;

            float gameTimeSecondsElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float movementAmount = MovementIncrement * gameTimeSecondsElapsed + 23;

            switch (Direction)
            {
                case SnakeDirection.Up:
                    Position = new Vector2(Position.X, Position.Y - movementAmount);
                    break;
                case SnakeDirection.Right:
                    Position = new Vector2(Position.X + movementAmount, Position.Y);
                    break;
                case SnakeDirection.Down:
                    Position = new Vector2(Position.X, Position.Y + movementAmount);
                    break;
                case SnakeDirection.Left:
                    Position = new Vector2(Position.X - movementAmount, Position.Y);
                    break;
            }

            Rotation = Direction.SnakeDirectionToRadius();

            // Reset frame count since last movement
            _framesSinceLastMovement = 0;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, Position, Rotation);
        }
    }
}
