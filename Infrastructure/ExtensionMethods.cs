using System;

namespace MonoSnake.Infrastructure
{
    public static class ExtensionMethods
    {
        public static float SnakeDirectionToRadius(this SnakeDirection snakeDirection)
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
    }
}
