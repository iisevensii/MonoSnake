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
        public SnakeHead SnakeHead { get; set; }
        private List<SnakeSegment> SnakeSegments { get; set; } = new List<SnakeSegment>();
        private int _segmentCount = 1;
        private readonly Sprite _snakeTailSprite;
        private readonly Sprite _straightBodySprite;
        private readonly Sprite _turn0Sprite;
        private readonly Sprite _turn1Sprite;
        private readonly Sprite _turn2Sprite;
        private readonly Sprite _turn3Sprite;
        Vector2 _lastSegmentPosition = Vector2.Zero;
        private float _lastSegmentRotation;
        private SnakeDirection _lastSegmentDirection;

        public Snake(SnakeHead snakeHead, Sprite snakeTailSprite, Sprite straightBodySprite,
            Sprite turn0Sprite, Sprite turn1Sprite, Sprite turn2Sprite, Sprite turn3Sprite)
        {
            SnakeHead = snakeHead;
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
            SnakeSegments.Add(new SnakeSegment(_snakeTailSprite, _lastSegmentPosition, _lastSegmentRotation, _lastSegmentDirection));
            SnakeSegments.Select(s => $"X: {s.Position.X}, Y: {s.Position.Y}, R: {s.Rotation}")
                .ToList()
                .ForEach(s => Trace.WriteLine(s));

        }

        public void Update(GameTime gameTime)
        {
            // See if SnakeHead is ready to be moved again
            if(!SnakeHead.CanUpdate())
                return;

            if (SnakeSegments.Count < _segmentCount)
            {
                if (SnakeSegments.Count == 0)
                {
                    _lastSegmentPosition = SnakeHead.Position;
                    _lastSegmentRotation = SnakeHead.Direction.ToRadius();
                    _lastSegmentDirection = SnakeDirection.Right;
                }

                AddSegment();
            }

            // Previous position of the SnakeHead before Update
            Vector2 previousSnakeHeadPosition = SnakeHead.Position;
            float previousSnakeHeadRotation = SnakeHead.Rotation;
            SnakeDirection previousSnakeHeadDirection = SnakeHead.Direction;

            // Move Snake Head
            SnakeHead.Update(gameTime);

            Vector2 previousPosition = Vector2.Zero;
            float previousRotation = 0f;
            SnakeDirection previousDirection = SnakeDirection.Right;

            for (int i = 0; i < SnakeSegments.Count; i++)
            {
                SnakeSegments[i].NoRotation = false;
                bool isTail = i == SnakeSegments.Count - 1;
                SnakeSegment previousSegment = null;
                SnakeSegment grandFatherSegment = null;
                SnakeSegment nextSegment = null;

                SnakeSegment currentSegment = SnakeSegments[i];

                if (isTail)
                {
                    currentSegment.Sprite = _snakeTailSprite;
                }
                else
                {
                    if (i > 0)
                        previousSegment = SnakeSegments[i - 1];

                    if (i > 1)
                        grandFatherSegment = SnakeSegments[i - 2];

                    currentSegment.Sprite = _straightBodySprite;
                    nextSegment = SnakeSegments[i + 1];
                    Trace.WriteLine($"i: {SnakeSegments.IndexOf(currentSegment)}, direction: {currentSegment.Direction}");
                    //if (previousSegment != null && nextSegment != null)
                    //{
                    //    var previousSegmentPosition = previousSegment.Position;
                    //    var nextSegmentPosition = nextSegment.Position;
                    //    if
                    //    (
                    //        (
                    //            currentSegmentPosition.X < previousSegmentPosition.X
                    //            &&
                    //            currentSegmentPosition.Y < nextSegmentPosition.Y
                    //            &&
                    //            Math.Abs(currentSegmentPosition.Y - previousSegmentPosition.Y) < 0.5f
                    //            &&
                    //            Math.Abs(currentSegmentPosition.X - nextSegmentPosition.X) < 0.5f
                    //        )
                    //    )
                    //    {
                    //        _snakeSegments[i].NoRotation = true;
                    //        _snakeSegments[i].Sprite = _turn0Sprite;
                    //        Trace.WriteLine($"We Have A Winner!!!!! I: {i}");
                    //    }
                    //    else
                    //    {
                    //        _snakeSegments[i].NoRotation = false;
                    //    }
                    //}
                }

                if (i == 0)
                {
                    previousPosition = currentSegment.Position;
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

                    previousPosition = currentSegment.Position;
                    previousRotation = currentSegment.Rotation;
                    previousDirection = currentSegment.Direction;

                    currentSegment.Position = positionToConsume;
                    currentSegment.Rotation = rotationToConsume;
                    currentSegment.Direction = directionToConsume;
                }

                //if (previousSegment != null && nextSegment != null)
                //{
                //    if (previousSegment.Direction == SnakeDirection.Right
                //        && currentSegment.Direction == SnakeDirection.Right
                //        && nextSegment.Direction == SnakeDirection.Up)
                //    {
                //        currentSegment.Rotation = 0f;
                //        currentSegment.Sprite = _turn0Sprite;
                //    }
                //    else
                //    {
                //        currentSegment.Sprite = _straightBodySprite;
                //        SnakeSegments[i].NoRotation = false;
                //    }
                //}
            }

            for (int i = 0; i < SnakeSegments.Count; i++)
            {
                SnakeSegment previousSegment = null;
                SnakeSegment grandFatherSegment = null;
                SnakeSegment nextSegment = null;
                SnakeDirection previousSegmentDirection;
                SnakeDirection currentSegmentDirection;
                SnakeDirection nextSegmentDirection;

                SnakeSegment currentSegment = SnakeSegments[i];
                currentSegmentDirection = currentSegment.Direction;

                if (i == 0)
                    previousSegmentDirection = SnakeHead.Direction;
                else
                    previousSegmentDirection = SnakeSegments[i - 1].Direction;

                if (i < SnakeSegments.Count -1)
                {
                    nextSegmentDirection = SnakeSegments[i + 1].Direction;

                    if 
                    (
                        previousSegmentDirection == SnakeDirection.Right
                        && currentSegmentDirection == SnakeDirection.Right
                        && nextSegmentDirection == SnakeDirection.Up
                        ||
                        previousSegmentDirection == SnakeDirection.Down
                        && currentSegmentDirection == SnakeDirection.Down
                        && nextSegmentDirection == SnakeDirection.Left
                        ||
                        previousSegmentDirection == SnakeDirection.Right
                        && currentSegmentDirection == SnakeDirection.Down
                        && nextSegmentDirection == SnakeDirection.Left
                        ||
                        previousSegmentDirection == SnakeDirection.Left
                        && currentSegmentDirection == SnakeDirection.Down
                        && nextSegmentDirection == SnakeDirection.Left
                    )
                    {
                        currentSegment.Rotation = 0f;
                        currentSegment.Sprite = _turn0Sprite;
                    }
                    else if 
                    (
                        previousSegmentDirection == SnakeDirection.Down
                        && currentSegmentDirection == SnakeDirection.Down
                        && nextSegmentDirection == SnakeDirection.Right
                        ||
                        previousSegmentDirection == SnakeDirection.Left
                        && currentSegmentDirection == SnakeDirection.Left
                        && nextSegmentDirection == SnakeDirection.Up
                        ||
                        previousSegmentDirection == SnakeDirection.Left
                        && currentSegmentDirection == SnakeDirection.Down
                        && nextSegmentDirection == SnakeDirection.Right
                        ||
                        previousSegmentDirection == SnakeDirection.Right
                        && currentSegmentDirection == SnakeDirection.Down
                        && nextSegmentDirection == SnakeDirection.Right
                    )
                    {
                        currentSegment.Rotation = 0f;
                        currentSegment.Sprite = _turn1Sprite;
                    }
                    else if
                    (
                        previousSegmentDirection == SnakeDirection.Left
                        && currentSegmentDirection == SnakeDirection.Left
                        && nextSegmentDirection == SnakeDirection.Down
                        ||
                        previousSegmentDirection == SnakeDirection.Up
                        && currentSegmentDirection == SnakeDirection.Up
                        && nextSegmentDirection == SnakeDirection.Right
                        ||
                        previousSegmentDirection == SnakeDirection.Left
                        && currentSegmentDirection == SnakeDirection.Up
                        && nextSegmentDirection == SnakeDirection.Right
                        ||
                        previousSegmentDirection == SnakeDirection.Right
                        && currentSegmentDirection == SnakeDirection.Up
                        && nextSegmentDirection == SnakeDirection.Right
                    )
                    {
                        currentSegment.Rotation = 0f;
                        currentSegment.Sprite = _turn2Sprite;
                    }
                    else if
                    (
                        previousSegmentDirection == SnakeDirection.Up
                        && currentSegmentDirection == SnakeDirection.Up
                        && nextSegmentDirection == SnakeDirection.Left
                        ||
                        previousSegmentDirection == SnakeDirection.Right
                        && currentSegmentDirection == SnakeDirection.Right
                        && nextSegmentDirection == SnakeDirection.Down
                        ||
                        previousSegmentDirection == SnakeDirection.Right
                        && currentSegmentDirection == SnakeDirection.Up
                        && nextSegmentDirection == SnakeDirection.Left
                        ||
                        previousSegmentDirection == SnakeDirection.Left
                        && currentSegmentDirection == SnakeDirection.Up
                        && nextSegmentDirection == SnakeDirection.Left
                    )
                    {
                        currentSegment.Rotation = 0f;
                        currentSegment.Sprite = _turn3Sprite;
                    }
                    else
                    {
                        currentSegment.Sprite = _straightBodySprite;
                        currentSegment.NoRotation = false;
                    }
                }
                else
                {
                    currentSegment.NoRotation = false;
                    currentSegment.Rotation = currentSegment.Direction.ToRadius();
                }
            }

            // Save last segment position so that new segments can use it as their starting position
            _lastSegmentPosition = SnakeSegments.Last().Position;

            //// Save last segment position so that new segments can use it as their starting position
            if (SnakeSegments.Count == 1)
            {
                _lastSegmentRotation = SnakeHead.Rotation;
                _lastSegmentDirection = SnakeHead.Direction;
            }
            else
            {
                _lastSegmentRotation = SnakeSegments.Last().Rotation;
                _lastSegmentDirection = SnakeSegments.Last().Direction;
            }

            // Temporary Code for testing
            if (_segmentCount < 9)
                _segmentCount++;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            SnakeHead.Draw(spriteBatch, gameTime);
            foreach (var s in SnakeSegments)
                s.Draw(spriteBatch, gameTime);
        }
    }
}
