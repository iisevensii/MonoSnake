using System;
using System.Collections.Generic;
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
            _snakeSegments.Add(new SnakeSegment(_snakeTailSprite, _lastSegmentPosition));
        }

        public void Update(GameTime gameTime)
        {
            if (_snakeSegments.Count < _segmentCount)
            {
                AddSegment();
            }

            // See if SnakeHead is ready to be moved again
            if(!_snakeHead.CanUpdate())
                return;

            // Previous position of the SnakeHead before Update
            Vector2 previousSnakeHeadPosition = _snakeHead.Position;

            // Move Snake Head
            _snakeHead.Update(gameTime);

            // For tracking the previous segment's position and applying to the current segment
            Vector2 previousSegmentPosition = Vector2.Zero;

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
                    snakeSegment.PreviousSnakeSegmentPosition = previousSnakeHeadPosition;
                    previousSegmentPosition = snakeSegment.PreviousSnakeSegmentPosition;
                }
                // All other segments follow the previous segment
                else
                {

                    // Position to be applied to the current segment
                    Vector2 newPosition = previousSegmentPosition;
                    // Position to be applied to the next segment (if it exists)
                    previousSegmentPosition = snakeSegment.Position;
                    // Set current segment position to that of the last segment
                    snakeSegment.PreviousSnakeSegmentPosition = newPosition;
                }

                // Apply Correct Sprite and Rotation based on the direction the current segment is heading
                snakeSegment.Sprite.Rotation = (float)(90 * Math.PI / 180);
                snakeSegment.Update(gameTime);
            }

            // Save last segment position so that new segments can use it as their starting position
            _lastSegmentPosition = _snakeSegments.Last().Position;
            
            // Temporary Code for testing
            if(_segmentCount < 9)
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
