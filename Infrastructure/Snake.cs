using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.GameObjects;

namespace MonoSnake.Infrastructure
{
    public class Snake
    {
        private readonly SnakeHead _snakeHead;
        private readonly List<SnakeSegment> _snakeSegments = new List<SnakeSegment>();
        private int _segmentCount = 1;
        private readonly Sprite _snakeTailSprite;
        private readonly Sprite _straightBodySprite;
        Vector2 _lastSegmentPosition = Vector2.Zero;
        private float _lastSegmentRotation;

        public Snake(SnakeHead snakeHead, Sprite snakeTailSprite, Sprite straightBodySprite)
        {
            _snakeHead = snakeHead;
            _snakeTailSprite = snakeTailSprite;
            _straightBodySprite = straightBodySprite;
        }

        private void Eat()
        {
            _segmentCount++;
        }

        private void AddSegment()
        {
            _snakeSegments.Add(new SnakeSegment(_snakeTailSprite, _lastSegmentPosition, _lastSegmentRotation));
            _snakeSegments.Select(s => $"X: {s.Position.X}, Y: {s.Position.Y}, R: {s.Rotation}")
                .ToList()
                .ForEach(s => Trace.WriteLine(s));

        }

        public void Update(GameTime gameTime)
        {
            // See if SnakeHead is ready to be moved again
            if(!_snakeHead.CanUpdate())
                return;

            if (_snakeSegments.Count < _segmentCount)
            {
                if (_snakeSegments.Count == 0)
                {
                    _lastSegmentPosition = _snakeHead.Position;
                    _lastSegmentRotation = _snakeHead.Direction.ToRadius();
                }

                AddSegment();
            }

            // Previous position of the SnakeHead before Update
            Vector2 previousSnakeHeadPosition = _snakeHead.Position;
            float previousSnakeHeadRotation = _snakeHead.Rotation;
            SnakeDirection previousSnakeHeadDirection = _snakeHead.Direction;

            // Move Snake Head
            _snakeHead.Update(gameTime);

            Vector2 previousPosition = Vector2.Zero;
            float previousRotation = 0f;
            SnakeDirection previousDirection = SnakeDirection.Right;

            for (int i = 0; i < _snakeSegments.Count; i++)
            {
                if (i == _snakeSegments.Count - 1)
                {
                    _snakeSegments[i].Sprite = _snakeTailSprite;
                }
                else
                    _snakeSegments[i].Sprite = _straightBodySprite;

                if (i == 0)
                {
                    previousPosition = _snakeSegments[i].Position;
                    previousRotation = _snakeSegments[i].Rotation;
                    previousDirection = _snakeSegments[i].Direction;

                    _snakeSegments[i].Position = previousSnakeHeadPosition;
                    _snakeSegments[i].Rotation = previousSnakeHeadRotation;
                    _snakeSegments[i].Direction = previousSnakeHeadDirection;
                }
                else
                {
                    Vector2 positionToConsume = previousPosition;
                    float rotationToConsume = previousRotation;
                    SnakeDirection directionToConsume = previousDirection;

                    previousPosition = _snakeSegments[i].Position;
                    previousRotation = _snakeSegments[i].Rotation;
                    previousDirection = _snakeSegments[i].Direction;

                    _snakeSegments[i].Position = positionToConsume;
                    _snakeSegments[i].Rotation = rotationToConsume;
                    _snakeSegments[i].Direction = directionToConsume;
                }
            }

            // Save last segment position so that new segments can use it as their starting position
            _lastSegmentPosition = _snakeSegments.Last().Position;

            //// Save last segment position so that new segments can use it as their starting position
            if (_snakeSegments.Count == 1)
                this._lastSegmentRotation = _snakeHead.Rotation;
            else
                _lastSegmentRotation = _snakeSegments.Last().Rotation;

            // Temporary Code for testing
            if (_segmentCount < 9)
                _segmentCount++;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _snakeHead.Draw(spriteBatch, gameTime);
            foreach (var s in _snakeSegments)
                s.Draw(spriteBatch, gameTime);
        }
    }
}
