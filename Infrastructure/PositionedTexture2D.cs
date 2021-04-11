using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSnake.Infrastructure
{
    public class PositionedTexture2D
    {
        public Texture2D SpriteSheet { get; set; }
        public int Margin { get; set; }
        public int SprintXIndex { get; set; }
        public int SpriteYIndex { get; set; }

        public PositionedTexture2D(Texture2D spriteSheet)
        {
            SpriteSheet = spriteSheet;
        }
        public PositionedTexture2D(Texture2D spriteSheet, int margin, int sprintXIndex, int spriteYIndex)
        {
            SpriteSheet = spriteSheet;
            Margin = margin;
            SprintXIndex = sprintXIndex;
            SpriteYIndex = spriteYIndex;
        }
    }
}
