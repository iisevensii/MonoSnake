using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoSnake.Infrastructure;

namespace MonoSnake.GameObjects
{
    public class AppleObject : IGameObject
    {
        public Sprite Sprite { get; }
        public Vector2 Position { get; set; }

        public AppleObject(Sprite sprite, Vector2 position)
        {
            Sprite = sprite;
            Position = position;
        }
    }
}
