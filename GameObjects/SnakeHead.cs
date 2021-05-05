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

        private float _movementSpeed;
        private int _framesSinceLastMovement = 20;
        private float _difficultyModifier = 1f;

        public float MovementSpeed
        {
            get
            {
                return _movementSpeed;
            }
            set
            {
                _movementSpeed = 20 - _difficultyModifier;
            }
        }

        public int DrawOrder => 0;
        public ISprite Sprite { get; }
        public Vector2 Position { get; set; }
        public Vector2 NextPosition { get; set; }
        public float Rotation { get; set; }
        public event Action<int> PointsScored;

        public SnakeDirection Direction { get; set; } = SnakeDirection.Right;

        public float DifficultyLevel
        {
            get => _difficultyModifier;
            set => _difficultyModifier = value;
        }


        public SnakeHead(ISprite sprite, Vector2 position)
        {
            Sprite = sprite;
            Position = position;
            Rotation = Direction.ToRadius();
            DifficultyLevel = 1;
            MovementSpeed = MovementSpeed;
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
