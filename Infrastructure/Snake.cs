using System;
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
        private readonly Sprite _turn0Sprite;
        private readonly Sprite _turn1Sprite;
        private readonly Sprite _turn2Sprite;
        private readonly Sprite _turn3Sprite;
        Vector2 _lastSegmentPosition = Vector2.Zero;
        private float _lastSegmentRotation;

        public Snake(SnakeHead snakeHead, Sprite snakeTailSprite, Sprite straightBodySprite,
            Sprite turn0Sprite, Sprite turn1Sprite, Sprite turn2Sprite, Sprite turn3Sprite)
        {
            _snakeHead = snakeHead;
            _snakeTailSprite = snakeTailSprite;
            _straightBodySprite = straightBodySprite;
            _turn0Sprite = turn0Sprite;
            _turn1Sprite = turn1Sprite;
            _turn2Sprite = turn2Sprite;
            _turn3Sprite = turn3Sprite;
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
                _snakeSegments[i].NoRotation = false;
                bool isTail = i == _snakeSegments.Count - 1;
                SnakeSegment previousSegment = null;
                SnakeSegment nextSegment = null;

                SnakeSegment currentSegment = _snakeSegments[i];
                var currentSegmentPosition = currentSegment.Position;
                if (isTail)
                {
                    currentSegment.Sprite = _snakeTailSprite;
                }
                else
                {
                    if (_snakeSegments.Count > 1 && i > 0)
                        previousSegment = _snakeSegments[i - 1];
                    currentSegment.Sprite = _straightBodySprite;
                    nextSegment = _snakeSegments[i + 1];


                    if (previousSegment != null && nextSegment != null)
                    {
                        var previousSegmentPosition = previousSegment.Position;
                        var nextSegmentPosition = nextSegment.Position;
                        if
                        (
                            (
                                currentSegmentPosition.X < previousSegmentPosition.X
                                &&
                                currentSegmentPosition.Y < nextSegmentPosition.Y
                                &&
                                Math.Abs(currentSegmentPosition.Y - previousSegmentPosition.Y) < 0.5f
                                &&
                                Math.Abs(currentSegmentPosition.X - nextSegmentPosition.X) < 0.5f
                            )
                        )
                        {
                            _snakeSegments[i].NoRotation = true;
                            _snakeSegments[i].Sprite = _turn0Sprite;
                            Trace.WriteLine($"We Have A Winner!!!!! I: {i}");
                        }
                        else
                        {
                            _snakeSegments[i].NoRotation = false;
                        }
                    }
                }

                if (i == 0)
                {
                    previousPosition = currentSegmentPosition;
                    previousRotation = currentSegment.Rotation;
                    previousDirection = currentSegment.Direction;

                    currentSegment.Position = previousSnakeHeadPosition;
                    currentSegment.Rotation = previousSnakeHeadRotation;
                    currentSegment.Direction = previousSnakeHeadDirection;
                }
                else
                {
                    Vector2 positionToConsume = previousPosition;
                    float rotationToConsume = previousRotation;
                    SnakeDirection directionToConsume = previousDirection;

                    previousPosition = currentSegmentPosition;
                    previousRotation = currentSegment.Rotation;
                    previousDirection = currentSegment.Direction;

                    currentSegment.Position = positionToConsume;
                    currentSegment.Rotation = rotationToConsume;
                    currentSegment.Direction = directionToConsume;
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
