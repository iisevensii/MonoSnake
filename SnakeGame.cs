using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoSnake.GameObjects;
using MonoSnake.Infrastructure;
using SharpDX.Direct2D1.Effects;

namespace MonoSnake
{
    public class SnakeGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _appleTexture;
        private Vector2 _applePosition;
        private float _appleSpeed;
        private int _screenWidth;
        private int _screenHeight;
        private SnakeHead _snakeHeadGameObject;

        private InputController _inputController;
        private Snake _snake;
        private Texture2D _snakeHeadSpriteSheet;
        private Texture2D _snakeSegmentsSpriteSheet;

        public SnakeGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _screenWidth = _graphics.PreferredBackBufferWidth;
            _screenHeight = _graphics.PreferredBackBufferHeight;
            _applePosition = new Vector2(_screenWidth / 2f, _screenHeight / 2f);
            _appleSpeed = 1000f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load Textures
            _appleTexture = Content.Load<Texture2D>("appletrans");
            _snakeHeadSpriteSheet = Content.Load<Texture2D>("snakehead");
            _snakeSegmentsSpriteSheet = Content.Load<Texture2D>("snakesegments");

            // Create Sprite Objects
            Sprite appleSprite = new Sprite(_appleTexture, 0, 0, _appleTexture.Width, _appleTexture.Height)
            {
                Origin = new Vector2(_appleTexture.Width / 2f, _appleTexture.Height / 2f)
            };
            Sprite snakeHeadSprite = new Sprite(_snakeHeadSpriteSheet, 0, 0, _snakeHeadSpriteSheet.Width, 42)
            {
                Origin = new Vector2(21, 21)
            };
            Sprite snakeTailSprite = new Sprite(_snakeSegmentsSpriteSheet, 84, 0, 42, 42)
            {
                Origin = new Vector2(21, 21)
            };

            // Create GameObjects
            _snakeHeadGameObject = new SnakeHead(snakeHeadSprite, new Vector2(100, 100));

            // Initialize Snake
            _snake = new Snake(_snakeHeadGameObject, snakeTailSprite);

            // Initialize InputController
            _inputController = new InputController(_snakeHeadGameObject);
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var player1GamePadState = GamePad.GetState(PlayerIndex.One);

            if (player1GamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            // Move Apple
            //MoveApple(gameTime, keyboardState, player1GamePadState);

            // Prevent Apple from leaving the screen
            KeepTextureOnScreen(_appleTexture, ref _applePosition);

            // Process Input
            _inputController.ProcessInput(gameTime);

            // Update GameObjects
            _snake.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            // Draw Apple
            //_appleSprite.Draw(_spriteBatch, _applePosition);
            _snake.Draw(_spriteBatch, gameTime);
            //_spriteBatch.Draw(_appleTexture, new Vector2(0, 0), null, Color.White, (float)Math.Atan2(sprite.Angle.Y, sprite.Angle.X), new Vector2(sprite.Area.Width / 2, sprite.Area.Height / 2), Scale, SpriteEffects.None, 0f);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void MoveApple(GameTime gameTime, KeyboardState keyboardState, GamePadState player1GamePadState)
        {
            if (keyboardState.IsKeyDown(Keys.Up) || player1GamePadState.IsButtonDown(Buttons.DPadUp))
                _applePosition.Y -= _appleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.Down) || player1GamePadState.IsButtonDown(Buttons.DPadDown))
                _applePosition.Y += _appleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.Left) || player1GamePadState.IsButtonDown(Buttons.DPadLeft))
                _applePosition.X -= _appleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.Right) || player1GamePadState.IsButtonDown(Buttons.DPadRight))
                _applePosition.X += _appleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void KeepTextureOnScreen(Texture2D texture2D, ref Vector2 position)
        {
            if (position.X > _screenWidth - texture2D.Width / 2f)
                position.X = _screenWidth - texture2D.Width / 2f;
            else if (position.X < texture2D.Width / 2f)
                position.X = texture2D.Width / 2f;

            if (position.Y > _screenHeight - texture2D.Height / 2f)
                position.Y = _screenHeight - texture2D.Height / 2f;
            else if (position.Y < texture2D.Height / 2f)
                position.Y = texture2D.Height / 2f;
        }
    }
}
