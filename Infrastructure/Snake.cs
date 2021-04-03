using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        private float _lastSegmentRotation = 0f;

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
            _snakeSegments.Select(s => $"X: {s.Position.X}, Y: {s.Position.Y}")
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
                    _lastSegmentRotation = _snakeHead.Rotation;
                }

                AddSegment();
            }

            // Previous position of the SnakeHead before Update
            Vector2 previousSnakeHeadPosition = _snakeHead.Position;
            float previousSnakeHeadRotation = _snakeHead.Rotation;

            // Move Snake Head
            _snakeHead.Update(gameTime);

            // For tracking the previous segment's position and applying to the current segment
            Vector2 previousSegmentPosition = Vector2.Zero;
            // For tracking the previous segment's position and applying to the current segment
            float previousSegmentRotation = 0f;

            // Move Segments
            foreach (SnakeSegment snakeSegment in _snakeSegments)
            {
                int currentIndex = _snakeSegments.IndexOf(snakeSegment);
                bool isFirstSegment = currentIndex == 0;
                bool isLastSegment = currentIndex + 1 == _snakeSegments.Count;

                if (isLastSegment)
                {
                    snakeSegment.Sprite = _snakeTailSprite;
                }
                else
                {
                    snakeSegment.Sprite = _straightBodySprite;
                }

                // Neck (First Segment - Follows SnakeHead directly)
                if (isFirstSegment)
                {
                    snakeSegment.PreviousSegmentPosition = previousSnakeHeadPosition;
                    previousSegmentPosition = snakeSegment.PreviousSegmentPosition;

                    snakeSegment.PreviousSegmentRotation = previousSnakeHeadRotation;
                    previousSegmentRotation = snakeSegment.PreviousSegmentRotation;
                }
                // All other segments follow the previous segment
                else
                {

                    // Position to be applied to the current segment
                    Vector2 newPosition = previousSegmentPosition;
                    // Position to be applied to the next segment (if it exists)
                    previousSegmentPosition = snakeSegment.Position;
                    // Set current segment position to that of the last segment
                    snakeSegment.PreviousSegmentPosition = newPosition;
                    // Set current segment direction to that of the last segment
                    snakeSegment.Direction = _snakeSegments[currentIndex - 1].Direction;

                    // Rotation to be applied to the current segment
                    float newRotation = previousSegmentRotation;
                    // Position to be applied to the next segment (if it exists)
                    previousSegmentRotation = snakeSegment.Rotation;
                    // Set current segment position to that of the last segment
                    snakeSegment.PreviousSegmentRotation = newRotation;
                    // Set current segment direction to that of the last segment
                    snakeSegment.Rotation = _snakeSegments[currentIndex - 1].Rotation;
                }

                snakeSegment.Update(gameTime);
            }

            // Save last segment position so that new segments can use it as their starting position
            _lastSegmentPosition = _snakeSegments.Last().Position;

            //// Save last segment position so that new segments can use it as their starting position
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
