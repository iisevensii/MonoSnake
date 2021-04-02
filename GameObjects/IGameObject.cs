using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoSnake.Infrastructure;

namespace MonoSnake.GameObjects
{
    interface IGameObject
    {
        Sprite Sprite { get; }

        Vector2 Position { get; set; }
    }
}
