﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSnake.GameObjects;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MonoSnake.Infrastructure
{
    public class Snake
    {
        public List<SnakeSegment> SnakeSegments { get; } = new List<SnakeSegment>();
        private int _segmentCount = 1;

        private readonly SpriteFont _scoreBoardFont;
        private readonly Sprite _snakeTailSprite;
        private readonly Sprite _straightBodySprite;
        private readonly Sprite _turn0Sprite;
        private readonly Sprite _turn1Sprite;
        private readonly Sprite _turn2Sprite;
        private readonly Sprite _turn3Sprite;

        private Vector2 _lastSegmentPosition = Vector2.Zero;
        private float _lastSegmentRotation;
        public SnakeDirection LastInputDirection { get; set; } = SnakeDirection.Right;
        private SnakeDirection _lastSegmentDirection;
        private readonly int _screenWidth;
        private readonly int _screenHeight;
        private int _score;

        public SnakeHead SnakeHead { get; }
        public int Score => _score;


        public Snake(SnakeHead snakeHead, int screenWidth, int screenHeight, SpriteFont scoreBoardFont, Sprite snakeTailSprite, Sprite straightBodySprite,
            Sprite turn0Sprite, Sprite turn1Sprite, Sprite turn2Sprite, Sprite turn3Sprite)
        {
            SnakeHead = snakeHead;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _scoreBoardFont = scoreBoardFont;
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

        public void AddSegment()
        {
            if (SnakeSegments.Count > 0)
                _score += 100;

            SnakeSegments.Add(new SnakeSegment(_snakeTailSprite, _lastSegmentPosition, _lastSegmentRotation, _lastSegmentDirection));
        }

        public void Update(GameTime gameTime)
        {
            // See if SnakeHead is ready to be moved again
            if(!SnakeHead.CanUpdate())
                return;

            SnakeHead.Direction = LastInputDirection;

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
                bool isFirstSegment = i == 0;
                bool isTail = i == SnakeSegments.Count - 1;
                SnakeSegment currentSegment = SnakeSegments[i];

                currentSegment.Sprite = isTail ? _snakeTailSprite : _straightBodySprite;

                if (isFirstSegment)
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
            }

            for (int i = 0; i < SnakeSegments.Count; i++)
            {
                SnakeDirection previousSegmentDirection;
                SnakeDirection currentSegmentDirection;
                SnakeDirection nextSegmentDirection;

                SnakeSegment currentSegment = SnakeSegments[i];

                currentSegmentDirection = currentSegment.Direction;
                previousSegmentDirection = i == 0 ? SnakeHead.Direction : SnakeSegments[i - 1].Direction;

                if (i < SnakeSegments.Count -1)
                {
                    nextSegmentDirection = SnakeSegments[i + 1].Direction;

                    /*
                     * Set correct sprite and rotation based on previous, current, and next segment directions
                     */
                    ApplyAppropriateSpriteAndRotation(ref currentSegment, previousSegmentDirection, currentSegmentDirection, nextSegmentDirection);
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
        }

        private void ApplyAppropriateSpriteAndRotation(ref SnakeSegment currentSegment, SnakeDirection previousSegmentDirection, SnakeDirection currentSegmentDirection, SnakeDirection nextSegmentDirection)
        {
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
                ||
                previousSegmentDirection == SnakeDirection.Down
                && currentSegmentDirection == SnakeDirection.Right
                && nextSegmentDirection == SnakeDirection.Up
                ||
                previousSegmentDirection == SnakeDirection.Up
                && currentSegmentDirection == SnakeDirection.Right
                && nextSegmentDirection == SnakeDirection.Up
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
                ||
                previousSegmentDirection == SnakeDirection.Down
                && currentSegmentDirection == SnakeDirection.Left
                && nextSegmentDirection == SnakeDirection.Up
                ||
                previousSegmentDirection == SnakeDirection.Up
                && currentSegmentDirection == SnakeDirection.Left
                && nextSegmentDirection == SnakeDirection.Up
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
                ||
                previousSegmentDirection == SnakeDirection.Up
                && currentSegmentDirection == SnakeDirection.Left
                && nextSegmentDirection == SnakeDirection.Down
                ||
                previousSegmentDirection == SnakeDirection.Down
                && currentSegmentDirection == SnakeDirection.Left
                && nextSegmentDirection == SnakeDirection.Down
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
                ||
                previousSegmentDirection == SnakeDirection.Up
                && currentSegmentDirection == SnakeDirection.Right
                && nextSegmentDirection == SnakeDirection.Down
                ||
                previousSegmentDirection == SnakeDirection.Down
                && currentSegmentDirection == SnakeDirection.Right
                && nextSegmentDirection == SnakeDirection.Down
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

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            SnakeHead.Draw(spriteBatch, gameTime);
            foreach (var s in SnakeSegments)
                s.Draw(spriteBatch, gameTime);

            string score = _score.ToString();
            float scoreScale = 2f;

            Vector2 scoreStringSize = _scoreBoardFont.MeasureString(score);
            Vector2 scorePos = new Vector2(_screenWidth - scoreStringSize.X *2 - 20, scoreStringSize.Y /2);
            spriteBatch.DrawString
            (
                _scoreBoardFont,
                score,
                scorePos,
                Color.White,
                0f,
                Vector2.Zero,
                scoreScale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
