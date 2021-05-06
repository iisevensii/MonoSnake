using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoSnake.Infrastructure
{
    public static class ExtensionMethods
    {
        public static float ToRadius(this SnakeDirection snakeDirection)
        {
            float rotation = 0f;
            switch (snakeDirection)
            {
                case SnakeDirection.Up:
                    rotation = (float)(180 * Math.PI / 180);
                    break;
                case SnakeDirection.Right:
                    rotation = (float)(270 * Math.PI / 180);
                    break;
                case SnakeDirection.Down:
                    rotation = 0f;
                    break;
                case SnakeDirection.Left:
                    rotation = (float)(90 * Math.PI / 180);
                    break;
            }

            return rotation;
        }

        public static List<Vector2> OutlinePixels(this Rectangle rectangle)
        {
            List<Vector2> pixels = new List<Vector2>();

            for (int x = rectangle.X; x < rectangle.X + rectangle.Width; x++)
            {
                for (int y = rectangle.Y; y < rectangle.Y + rectangle.Height; y++)
                {
                    if (y == rectangle.Y || y == rectangle.Y + rectangle.Height - 1)
                        pixels.Add(new Vector2(x, y));
                    if (x == rectangle.X || x == rectangle.X + rectangle.Width - 1)
                        pixels.Add(new Vector2(x, y));
                }
            }

            return pixels;
        }

        public static bool IsAlphabetKey(this Keys key)
        {
            return key >= Keys.A && key <= Keys.Z;
        }

        public static bool IsTopRowNumberKey(this Keys key)
        {
            return key >= Keys.D0 && key <= Keys.D9;
        }

        public static bool IsNumPadNumberKey(this Keys key)
        {
            return key >= Keys.NumPad0 && key <= Keys.NumPad9;
        }

    }
}
