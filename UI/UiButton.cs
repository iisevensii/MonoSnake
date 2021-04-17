using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoSnake.Infrastructure;

namespace MonoSnake.UI
{
    public enum UIButtonState
    {
        Normal,
        Hover,
        MouseDown
    }

    public class UiButton : IUiObject
    {
        private UIButtonState _buttonState = UIButtonState.Normal;
        private Rectangle _buttonRectangle;
        public MouseState OldUIMouseState { get; set; }
        public MouseState NewUIMouseState { get; set; }

        public int DrawOrder { get; }
        public Sprite Sprite { get; set; }
        public Sprite NormalStateSprite { get; set; }
        public Sprite HoverStateSprite { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle MouseRectangle { get; set; }
        public bool IsMouseOver { get; set; }
        public EventHandler ClickedEvent;

        public UiButton(Sprite normalStateSprite, Sprite hoverStateSprite, Vector2 position, float rotation)
        {
            Sprite = normalStateSprite;
            Sprite = normalStateSprite;
            Width = normalStateSprite.Width;
            Height = normalStateSprite.Height;
            HoverStateSprite = hoverStateSprite;
            Position = position;
            Rotation = rotation;
            NormalStateSprite = normalStateSprite;
            HoverStateSprite = hoverStateSprite;
        }

        protected virtual void OnClick(EventArgs e)
        {
            EventHandler handler = this.ClickedEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void UpdateButtonState()
        {
            _buttonState = !_buttonRectangle.Intersects(MouseRectangle) ? UIButtonState.Normal : UIButtonState.Hover;

            switch (_buttonState)
            {
                case UIButtonState.Normal:
                    IsMouseOver = false;
                    break;
                case UIButtonState.Hover:
                    IsMouseOver = true;
                    break;
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            OldUIMouseState = NewUIMouseState;
            NewUIMouseState = Mouse.GetState();

            _buttonRectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

            MouseRectangle = new Rectangle(NewUIMouseState.X, NewUIMouseState.Y, 1, 1);

            UpdateButtonState();

            switch (_buttonState)
            {
                case UIButtonState.Normal:
                    Sprite = NormalStateSprite;
                    break;
                case UIButtonState.Hover:
                    Sprite = HoverStateSprite;
                    break;
            }


            if (IsMouseOver && OldUIMouseState.LeftButton == ButtonState.Pressed && NewUIMouseState.LeftButton == ButtonState.Released)
            {
                OnClick(EventArgs.Empty);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, Position, Rotation);
        }
    }
}
