using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.Infrastructure;

namespace MonoSnake.GameObjects
{
    public class SnakeHead : IGameObject
    {
        private const float MOVEMENT_PER_FRAME = 40f;
        private const float MOVEMENT_INCREMENT = 1000f;

        private int _framesSinceLastMovement = 20;
        public int MovementSpeed { get; set; } = 18;
        public int DrawOrder => 0;
        public Sprite Sprite { get; }
        public Vector2 Position { get; set; }
        public Vector2 NextPosition { get; set; }
        public float Rotation { get; set; }

        public SnakeDirection Direction { get; set; } = SnakeDirection.Right;


        public SnakeHead(Sprite sprite, Vector2 position)
        {
            Sprite = sprite;
            Position = position;
            Rotation = Direction.ToRadius();
        }

        public bool CanUpdate()
        {
            // Update Frame Counter
            _framesSinceLastMovement++;

            // Require at least 20 frames to have passed before moving.
            // Direction from InputController is already set so it is not being lost
            if (_framesSinceLastMovement < MovementSpeed)
                return false;

            return true;
        }

        public void Update(GameTime gameTime)
        {
            if (!CanUpdate())
                return;

            switch (Direction)
            {
                case SnakeDirection.Up:
                    Position = new Vector2(Position.X, (float) Math.Round(Position.Y - MOVEMENT_PER_FRAME));
                    NextPosition = new Vector2(Position.X, Position.Y - MOVEMENT_PER_FRAME);
                    break;
                case SnakeDirection.Right:
                    Position = new Vector2((float) Math.Round(Position.X + MOVEMENT_PER_FRAME), Position.Y);
                    NextPosition = new Vector2(Position.X + MOVEMENT_PER_FRAME, Position.Y);
                    break;
                case SnakeDirection.Down:
                    Position = new Vector2(Position.X, (float) Math.Round(Position.Y + MOVEMENT_PER_FRAME));
                    NextPosition = new Vector2(Position.X, Position.Y + MOVEMENT_PER_FRAME);
                    break;
                case SnakeDirection.Left:
                    Position = new Vector2((float) Math.Round(Position.X - MOVEMENT_PER_FRAME), Position.Y);
                    NextPosition = new Vector2(Position.X - MOVEMENT_PER_FRAME, Position.Y);
                    break;
            }

            Rotation = Direction.ToRadius();

            // Reset frame count since last movement
            _framesSinceLastMovement = 0;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, Position, Rotation);
        }
    }
}
