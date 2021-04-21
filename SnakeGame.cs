using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoSnake.GameObjects;
using MonoSnake.Infrastructure;
using MonoSnake.UI;

namespace MonoSnake
{
    public class SnakeGame : Game
    {
        private enum UiState
        {
            GamePlay,
            StartScreen,
            HighScoresScreen
        }

        #region Constants
        private const int SCREEN_WIDTH = 780;
        private const int SCREEN_HEIGHT = 820;
        private const string LEADERBOARD_STRING = "L E A D E R B O A R D";
        private const int LEADERBOARD_MINIMUM_LENGTH = 10;
        private const string MONO_SNAKE_STRING = "MonoSnake";
        private const int START_SCREEN_TRANSPARENCY = 200;
        private const string GAME_OVER_STRING = "Game Over";
        private const float GAME_OVER_FONT_SCALE = 1f;
        private const int HIT_BOX_PADDING = 5;
        private const int DEFAULT_SPRITE_SIZE = 40;
        private const int DEFAULT_SPRITE_HALF_SIZE = 20;
        private const string APPLE_SPRITE_SHEET_NAME = "Apple";
        private const string SNAKE_HEAD_SPRITE_SHEET_NAME = "SnakeHead";
        private const string SNAKE_SEGMENTS_SPRITE_SHEET_NAME = "SnakeSegments";
        private const string EAT_SOUND_EFFECT_NAME = "eat";
        private const string HISS_SOUND_EFFECT_NAME = "hiss";
        private const string MOVE_SOUND_EFFECT_NAME = "sand_rattle";
        private const int SCORE_BOARD_FONT_LEFT_PADDING = 10;
        private const int GAME_AREA_MARGIN_LEFT = 20;
        private const int GAME_AREA_MARGIN_TOP = 90;
        private const int GAME_AREA_PADDING = 10;
        #endregion Constants

        #region Fields
            // Draw diagnostic grid?
            private readonly bool _drawDiagnosticGrid = false;
            private bool _appleEaten;
            private bool _applePlaced;
            private bool _isGameOver;
            private readonly bool _showFpsMonitor = false;

            #region System
            private readonly GraphicsDeviceManager _graphics;
            private SpriteBatch _spriteBatch;
            private InputController _inputController;
            private readonly FrameCounter _frameCounter = new FrameCounter();
        #endregion System

        #region UI
            private UiState _uiState;
            private UiFrame _startScreenUiFrame;
            private CenteredUiFrame _highScoresUiFrame;
            private ScoreBoard _scoreBoard;
            #endregion UI

            #region Sprites
            private Sprite _snakeCwUpToRightCcwLeftToDownSprite;
            private Sprite _snakeCwRightToDownCcwUpToLeftSprite;
            private Sprite _snakeCwDownToLeftCcwRightToUpSprite;
            private Sprite _snakeCwLeftToUpCcwDownToRightSprite;
            private Sprite _snakeStraightBodySprite;
            #endregion Sprites

            #region GameObjects
            private Snake _snake;
            private Apple _appleGameObject;
            private SnakeHead _snakeHeadGameObject;
            private ToggleUiButton _startScreenHighScoresToggleButton;
            #endregion GameObjects

            #region SpriteFonts
            private SpriteFont _logoFont;
            private SpriteFont _gameOverFont;
            private SpriteFont _leaderboardFont;
            private SpriteFont _scoreBoardFont;
            #endregion SpriteFonts

            #region Textures
            private Texture2D _appleTexture;
            private Texture2D _snakeHeadSpriteSheet;
            private Texture2D _snakeSegmentsSpriteSheet;
            private Texture2D _gameAreaRectangleTexture;
            private Texture2D _snakeHeadRectangleTexture;
            private Texture2D _startScreenButtonNormal;
            private Texture2D _startScreenButtonHover;
            private Texture2D _highScoresButtonNormal;
            private Texture2D _highScoresButtonHover;
            #endregion Textures

            #region SoundEffects
            private SoundEffect _eatSoundEffect;
            private readonly Random _randomSounds = new Random();
            private SoundEffect[] _moveSoundEffects;
            #endregion SoundEffects

            #region Rectangles
            private Rectangle _gameAreaRectangle;
            private Rectangle _snakeHeadRectangle;
            private Rectangle _appleRectangle;
            private List<Rectangle> _cells;
            private List<Rectangle> _occupiedCells = new List<Rectangle>();
            private List<Rectangle> _unOccupiedCells = new List<Rectangle>();
            private ISprite _snakeHeadAnimatedSprite;

            #endregion Rectangles
        #endregion Fields

        public SnakeGame()  
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #region Initialization

        protected override void Initialize()
        {
            _scoreBoard = new ScoreBoard(Assembly.GetEntryAssembly().Location);
            _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadAssets();

            InitializeGameObjects();

            InitializeDiagnosticObjects();

            GenerateGrid();

            GenerateApple();

            InitializeInputController();

            InitializeUiObjects();

            _uiState = UiState.StartScreen;
        }

        private void LoadAssets()
        {
            // Load Fonts
            _logoFont = Content.Load<SpriteFont>("Logo");
            _gameOverFont = Content.Load<SpriteFont>("GameOver");
            _leaderboardFont = Content.Load<SpriteFont>("Arcade");
            _scoreBoardFont = Content.Load<SpriteFont>("ArcadeClassic");
            // Load Textures
            _appleTexture = Content.Load<Texture2D>(APPLE_SPRITE_SHEET_NAME);
            _snakeHeadSpriteSheet = Content.Load<Texture2D>(SNAKE_HEAD_SPRITE_SHEET_NAME);
            _snakeSegmentsSpriteSheet = Content.Load<Texture2D>(SNAKE_SEGMENTS_SPRITE_SHEET_NAME);
            _eatSoundEffect = Content.Load<SoundEffect>(EAT_SOUND_EFFECT_NAME);
            _startScreenButtonNormal = Content.Load<Texture2D>("Gear");
            _startScreenButtonHover = Content.Load<Texture2D>("GearHover");
            _highScoresButtonNormal = Content.Load<Texture2D>("Ranking");
            _highScoresButtonHover = Content.Load<Texture2D>("RankingHover");
            _moveSoundEffects = new SoundEffect[] {
                Content.Load<SoundEffect>(HISS_SOUND_EFFECT_NAME),
                Content.Load<SoundEffect>(MOVE_SOUND_EFFECT_NAME)
            };
        }

        private void InitializeGameObjects()
        {
            // Create Sprite Objects
            Sprite appleSprite = new Sprite(_appleTexture, 0, 0, _appleTexture.Width, _appleTexture.Height)
            {
                Origin = new Vector2(_appleTexture.Width / 2f, _appleTexture.Height / 2f)
            };

            #region PositionedTextureSprites

            PositionedTexture2D headPositionedTexture2D = new PositionedTexture2D(_snakeHeadSpriteSheet, 1, 0, 0);
            PositionedTexture2D headPositionedTexture2D01 = new PositionedTexture2D(_snakeHeadSpriteSheet, 1, 0, 1);
            PositionedTexture2D headPositionedTexture2D02 = new PositionedTexture2D(_snakeHeadSpriteSheet, 1, 0, 2);

            Vector2 halfSizeOrigin = new Vector2(DEFAULT_SPRITE_HALF_SIZE, DEFAULT_SPRITE_HALF_SIZE);

            ISprite snakeHeadSprite = new Sprite(headPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = halfSizeOrigin
            };
            Sprite snakeHeadSprite01 = new Sprite(headPositionedTexture2D01, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = halfSizeOrigin
            };
            Sprite snakeHeadSprite02 = new Sprite(headPositionedTexture2D02, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = halfSizeOrigin
            };

            List<AnimatedSprite.AnimatedSpriteFrame> animatedSnakeHeadFrames = new List<AnimatedSprite.AnimatedSpriteFrame>
            {
                new AnimatedSprite.AnimatedSpriteFrame(snakeHeadSprite, 0.5f),
                new AnimatedSprite.AnimatedSpriteFrame(snakeHeadSprite01, 0.5f),
                new AnimatedSprite.AnimatedSpriteFrame(snakeHeadSprite02, 0.5f)
            };

            _snakeHeadAnimatedSprite = new AnimatedSprite(animatedSnakeHeadFrames, 0, 0, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE);

            PositionedTexture2D tailPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 0, 2);
            Sprite snakeTailSprite = new Sprite(tailPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = halfSizeOrigin
            };

            PositionedTexture2D straightBodyPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 1, 2);
            _snakeStraightBodySprite = new Sprite(straightBodyPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = halfSizeOrigin
            };

            PositionedTexture2D snakeCwUpToRightCcwLeftToDownPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 0, 0);
            _snakeCwUpToRightCcwLeftToDownSprite = new Sprite(snakeCwUpToRightCcwLeftToDownPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = halfSizeOrigin
            };

            PositionedTexture2D snakeCwRightToDownCcwUpToLeftPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 1, 0);
            _snakeCwRightToDownCcwUpToLeftSprite = new Sprite(snakeCwRightToDownCcwUpToLeftPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = halfSizeOrigin
            };

            PositionedTexture2D snakeCwDownToLeftCcwRightToUpPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 1, 1);
            _snakeCwDownToLeftCcwRightToUpSprite = new Sprite(snakeCwDownToLeftCcwRightToUpPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = halfSizeOrigin
            };

            PositionedTexture2D snakeCwLeftToUpCcwDownToRightPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 0, 1);
            _snakeCwLeftToUpCcwDownToRightSprite = new Sprite(snakeCwLeftToUpCcwDownToRightPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = halfSizeOrigin
            };

            #endregion PositionedTextureSprites

            // Create GameObjects
            _snakeHeadGameObject = new SnakeHead(_snakeHeadAnimatedSprite, 
                new Vector2(GAME_AREA_MARGIN_LEFT + GAME_AREA_PADDING + DEFAULT_SPRITE_HALF_SIZE,
                GAME_AREA_MARGIN_TOP + GAME_AREA_PADDING + DEFAULT_SPRITE_HALF_SIZE));

            _appleGameObject = new Apple(appleSprite,
                new Vector2(_graphics.PreferredBackBufferWidth / 2f + GAME_AREA_PADDING + DEFAULT_SPRITE_HALF_SIZE,
                    _graphics.PreferredBackBufferHeight / 2f + GAME_AREA_PADDING + DEFAULT_SPRITE_HALF_SIZE));

            _appleGameObject.Sprite.Scale = new Vector2(0.72f, 0.72f);

            // Initialize Snake
            _snake = new Snake
            (
                _snakeHeadGameObject,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight,
                _scoreBoardFont,
                snakeTailSprite,
                _snakeStraightBodySprite,
                _snakeCwUpToRightCcwLeftToDownSprite,
                _snakeCwRightToDownCcwUpToLeftSprite,
                _snakeCwDownToLeftCcwRightToUpSprite,
                _snakeCwLeftToUpCcwDownToRightSprite
            );
        }

        private void OnStartScreenHighScoresToggleButtonClick(object sender, EventArgs e)
        {
            if (_uiState != UiState.GamePlay || _isGameOver)
            {
                _startScreenHighScoresToggleButton.IsEnabled = true;
                _uiState = _startScreenHighScoresToggleButton.IsToggled ? UiState.HighScoresScreen : UiState.StartScreen;
            }
            else
            {
                _startScreenHighScoresToggleButton.IsEnabled = false;
            }
        }

        private void InitializeDiagnosticObjects()
        {
            _gameAreaRectangleTexture = new Texture2D(GraphicsDevice, 2, 2);
            _snakeHeadRectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            _gameAreaRectangleTexture.SetData(new[] { Color.White, Color.White, Color.White, Color.White, });
            _snakeHeadRectangleTexture.SetData(new[] { Color.White });

            _gameAreaRectangle = new Rectangle
            (
                GAME_AREA_MARGIN_LEFT,
                GAME_AREA_MARGIN_TOP,
                _graphics.PreferredBackBufferWidth - GAME_AREA_MARGIN_LEFT * 2,
                _graphics.PreferredBackBufferHeight - 120
            );
            _snakeHeadRectangle = new Rectangle
            (
                (int)Math.Round(_snake.SnakeHead.Position.X -
                                _snake.SnakeHead.Sprite.Width / 2f * _snake.SnakeHead.Sprite.Scale.X) - HIT_BOX_PADDING,
                (int)Math.Round(_snake.SnakeHead.Position.Y -
                                _snake.SnakeHead.Sprite.Height / 2f * _snake.SnakeHead.Sprite.Scale.Y) - HIT_BOX_PADDING,
                (int)(_snake.SnakeHead.Sprite.Width * _snake.SnakeHead.Sprite.Scale.X),
                (int)(_snake.SnakeHead.Sprite.Height * _snake.SnakeHead.Sprite.Scale.Y)
            );
        }

        #endregion Initialization

        #region Input

        private void InitializeInputController()
        {
            _inputController = new InputController(_snake);
            _inputController.StartEvent += InputController_StartEvent;
            _inputController.ExitEvent += InputController_ExitEvent;
            _inputController.RestartEvent += InputController_RestartEvent;
            _inputController.HeadTurnEvent += InputController_HeadTurnEvent;
        }

        private void InitializeUiObjects()
        {
            Sprite startScreenButtonNormal = new Sprite(_startScreenButtonNormal, 0, 0, 57, 57);
            Sprite startScreenButtonHover = new Sprite(_startScreenButtonHover, 0, 0, 57, 57);
            Sprite highScoresButtonNormal = new Sprite(_highScoresButtonNormal, 0, 0, 57, 57);
            Sprite highScoresButtonHover = new Sprite(_highScoresButtonHover, 0, 0, 57, 57);

            _startScreenHighScoresToggleButton =
                new ToggleUiButton
                    (
                        highScoresButtonNormal,
                        highScoresButtonHover,
                        startScreenButtonNormal,
                        startScreenButtonHover,
                        new Vector2(10, 10),
                        0f
                    );
            _startScreenHighScoresToggleButton.IsEnabled = true;
            _startScreenHighScoresToggleButton.ClickEvent += OnStartScreenHighScoresToggleButtonClick;

            _highScoresUiFrame = new CenteredUiFrame
            (
                _graphics,
                Vector2.Zero,
                17,
                17,
                SCREEN_WIDTH,
                SCREEN_HEIGHT,
                _snakeStraightBodySprite,
                _snakeStraightBodySprite,
                _snakeCwUpToRightCcwLeftToDownSprite,
                _snakeCwRightToDownCcwUpToLeftSprite,
                _snakeCwDownToLeftCcwRightToUpSprite,
                _snakeCwLeftToUpCcwDownToRightSprite,
                Color.FromNonPremultiplied(46, 51, 106, START_SCREEN_TRANSPARENCY)
            );

            _startScreenUiFrame = new CenteredUiFrame
            (
                _graphics,
                Vector2.Zero,
                17,
                17,
                SCREEN_WIDTH,
                SCREEN_HEIGHT,
                _snakeStraightBodySprite,
                _snakeStraightBodySprite,
                _snakeCwUpToRightCcwLeftToDownSprite,
                _snakeCwRightToDownCcwUpToLeftSprite,
                _snakeCwDownToLeftCcwRightToUpSprite,
                _snakeCwLeftToUpCcwDownToRightSprite,
                Color.FromNonPremultiplied(46, 51, 106, START_SCREEN_TRANSPARENCY)
            );
        }

        private void InputController_StartEvent(object sender, EventArgs e)
        {
            InitializeGameObjects();
            InitializeInputController();
            GenerateGrid();
            _appleEaten = false;
            _applePlaced = false;
            GenerateApple();
            _isGameOver = false;
        }

        private void InputController_RestartEvent(object sender, EventArgs e)
        {
            InitializeGameObjects();
            InitializeInputController();
            GenerateGrid();
            _uiState = UiState.GamePlay;
            _appleEaten = false;
            _applePlaced = false;
            GenerateApple();
            _isGameOver = false;
            _startScreenHighScoresToggleButton.IsEnabled = false;
        }

        private void InputController_ExitEvent(object sender, EventArgs e)
        {
            Exit();
        }

        private void InputController_HeadTurnEvent(object sender, EventArgs e)
        {
            if (_moveSoundEffects != null)
            {
                int roll = _randomSounds.Next(1, 101);
                if (roll <= 55) //55% chance
                {
                    //pick either a hiss or a sand shuffle (or whatever else we put in the array)
                    int i = _randomSounds.Next(0, _moveSoundEffects.Length);
                    //randomly changing the pitch is great way to trick
                    //player's into not hating the same sound repeated
                    double randomAmplify = _randomSounds.NextDouble() - 0.5;

                    _moveSoundEffects[i].Play(1f, (float)randomAmplify, 0f);
                }
            }
        }

        #endregion Input

        private void GenerateGrid()
        {
            // Draw Grid (Temp)
            _cells = new List<Rectangle>();
            //Occupied Cells
            _occupiedCells = new List<Rectangle>();
            // Columns
            // Rows
            for (int i = 0; i < 18; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    Rectangle rectangle = new Rectangle
                        (
                            i * DEFAULT_SPRITE_SIZE + GAME_AREA_MARGIN_LEFT + GAME_AREA_PADDING,
                            j * DEFAULT_SPRITE_SIZE + GAME_AREA_MARGIN_TOP + GAME_AREA_PADDING,
                            DEFAULT_SPRITE_SIZE,
                            DEFAULT_SPRITE_SIZE
                        );
                    _cells.Add(rectangle);
                }
            }

            foreach (Rectangle cell in _cells)
            {
                if (Math.Round(_snake.SnakeHead.Position.X) == cell.X + DEFAULT_SPRITE_HALF_SIZE &&
                    Math.Round(_snake.SnakeHead.Position.Y) == cell.Y + DEFAULT_SPRITE_HALF_SIZE)
                {
                    _occupiedCells.Add(cell);
                }

                if (_snake.SnakeSegments.Any(s =>
                    Math.Round(s.Position.X) == cell.X + DEFAULT_SPRITE_HALF_SIZE && Math.Round(s.Position.Y) == cell.Y + DEFAULT_SPRITE_HALF_SIZE))
                {
                    _occupiedCells.Add(cell);
                }
            }

            _unOccupiedCells = _cells.Except(_occupiedCells).ToList();
        }

        private void GenerateApple()
        {
            Random randomApple = new Random();

            int nextIndex = randomApple.Next(0, _unOccupiedCells.Count);

            if (!_applePlaced)
            {
                if (_unOccupiedCells.Count > 0)
                {
                    Vector2 nextAppleLoc = new Vector2(_unOccupiedCells[nextIndex].X + DEFAULT_SPRITE_HALF_SIZE, _unOccupiedCells[nextIndex].Y + DEFAULT_SPRITE_HALF_SIZE);

                    _appleGameObject.Position = nextAppleLoc;
                }

                _appleRectangle = new Rectangle
                (
                    (int)Math.Round(_appleGameObject.Position.X -
                                     _appleGameObject.Sprite.Width / 2f * _appleGameObject.Sprite.Scale.X) - HIT_BOX_PADDING,
                    (int)Math.Round(_appleGameObject.Position.Y -
                                     _appleGameObject.Sprite.Height / 2f * _appleGameObject.Sprite.Scale.Y) - HIT_BOX_PADDING,
                    (int)(_appleGameObject.Sprite.Width * _appleGameObject.Sprite.Scale.X) + HIT_BOX_PADDING * 2,
                    (int)(_appleGameObject.Sprite.Height * _appleGameObject.Sprite.Scale.Y) + HIT_BOX_PADDING * 2
                );

                _applePlaced = true;
                _appleEaten = false;
            }
        }

        private void EndGameAndRecordScore()
        {
            _scoreBoard.HighScores.AddHighScore(new ScoreEntry("SeVeN", _snake.Score));

            _scoreBoard.SaveHighScores();

            _isGameOver = true;

            _startScreenHighScoresToggleButton.IsEnabled = true;
        }

        #region UIDrawMethods

        private void DrawStartScreenUiFrame(GameTime gameTime)
        {
            _startScreenUiFrame.Draw(_spriteBatch, gameTime);
        }

        private void DrawHighScoresUiFrame(GameTime gameTime)
        {
            _highScoresUiFrame.Draw(_spriteBatch, gameTime);
        }

        private void DrawHighScores(GameTime gameTime)
        {
            int scoreBoardHorizontalMargin = 20;
            int scoreBoardVerticalMargin = 20;

            //float leaderboardX = _highScoresUiFrame.Position.X + _highScoresUiFrame.ActualWidth / 2 - leaderboardStringWidth.X / 2 - _snakeCwUpToRightCcwLeftToDownSprite.Width / 2;
            //float leaderboardY = _highScoresUiFrame.Position.Y / 2 + leaderboardStringWidth.Y + _snakeCwUpToRightCcwLeftToDownSprite.Width / 2;

            int scoreVerticalSpacing = 50;
            int scoresMarginTop = 50;
            foreach (ScoreEntry scoreEntry in _scoreBoard.HighScores.ScoreEntries)
            {
                int margin = (SCREEN_WIDTH - _startScreenUiFrame.ActualWidth) / 2;
                int leftInsideEdgeOfFrame = scoreBoardHorizontalMargin + margin + SCORE_BOARD_FONT_LEFT_PADDING + DEFAULT_SPRITE_SIZE / 2;
                int rightInsideEdgeOfFrame = SCREEN_WIDTH - scoreBoardHorizontalMargin - margin - SCORE_BOARD_FONT_LEFT_PADDING - DEFAULT_SPRITE_SIZE / 2;

                string scoreEntryName = scoreEntry.Name ?? "";
                string scoreText = scoreEntry.Score.ToString();

                scoreEntryName = scoreEntryName.PadRight(100, ' ');
                Vector2 scoreEntryNameScale = _scoreBoardFont.MeasureString(scoreEntryName);
                Vector2 scoreEntryScoreScale = _scoreBoardFont.MeasureString(scoreText);
                rightInsideEdgeOfFrame = rightInsideEdgeOfFrame - (int) scoreEntryScoreScale.X;

                float scoreEntryNameX = leftInsideEdgeOfFrame;
                var scoreEntryIndex = _scoreBoard.HighScores.ScoreEntries.IndexOf(scoreEntry);
                var frameYPosition = _highScoresUiFrame.Position.Y;
                float scoreEntryNameY = scoreBoardVerticalMargin  + frameYPosition + scoresMarginTop + scoreVerticalSpacing + scoreEntryIndex * scoreVerticalSpacing;
                float scoreEntryScoreX = rightInsideEdgeOfFrame;
                float scoreEntryScoreY = scoreBoardVerticalMargin + frameYPosition + scoresMarginTop + scoreVerticalSpacing + scoreEntryIndex * scoreVerticalSpacing;

                Vector2 scoreEntryNamePosition = new Vector2(scoreEntryNameX, scoreEntryNameY);
                Vector2 scoreEntryScorePosition = new Vector2(scoreEntryScoreX, scoreEntryScoreY);

                if(scoreEntryName.Length > 0)
                    _spriteBatch.DrawString
                    (
                        _scoreBoardFont,
                        scoreEntryName,
                        scoreEntryNamePosition,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0f
                    );

                if(scoreEntry.Score > 0)
                    _spriteBatch.DrawString
                    (
                        _scoreBoardFont,
                        scoreText,
                        scoreEntryScorePosition,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
            }
        }

        private void DrawLogoText()
        {
            Vector2 logoStringWidth = _logoFont.MeasureString(MONO_SNAKE_STRING);
            float logoX = _startScreenUiFrame.Position.X + _startScreenUiFrame.ActualWidth /2 - logoStringWidth.X / 2 - _snakeCwUpToRightCcwLeftToDownSprite.Width /2;
            float logoY = _startScreenUiFrame.Position.Y / 2 + logoStringWidth.Y + DEFAULT_SPRITE_HALF_SIZE;
            Vector2 logoPosition = new Vector2(logoX, logoY);

            _spriteBatch.DrawString
            (
                _logoFont,
                MONO_SNAKE_STRING,
                logoPosition,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );
        }

        private void DrawLeaderboardText()
        {
            _leaderboardFont.Spacing = 0f;
            Vector2 leaderboardStringWidth = _leaderboardFont.MeasureString(LEADERBOARD_STRING);
            float leaderboardX = _highScoresUiFrame.Position.X + _highScoresUiFrame.ActualWidth / 2 - leaderboardStringWidth.X / 2 - _snakeCwUpToRightCcwLeftToDownSprite.Width / 2;
            float leaderboardY = _highScoresUiFrame.Position.Y / 2 + leaderboardStringWidth.Y + _snakeCwUpToRightCcwLeftToDownSprite.Width / 2 + DEFAULT_SPRITE_HALF_SIZE;
            Vector2 leaderboardPosition = new Vector2(leaderboardX, leaderboardY);

            _spriteBatch.DrawString
            (
                _leaderboardFont,
                LEADERBOARD_STRING,
                leaderboardPosition,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );
        }

        private void DrawGameOverText()
        {
            Vector2 gameOverStringWidth = _gameOverFont.MeasureString(GAME_OVER_STRING) * GAME_OVER_FONT_SCALE;
            float gameOverX = SCREEN_WIDTH / 2 - gameOverStringWidth.X / 2;
            float gameOverY = SCREEN_HEIGHT / 2 - gameOverStringWidth.Y / 2;
            Vector2 gameOverPosition = new Vector2(gameOverX, gameOverY);

            _spriteBatch.DrawString
            (
                _gameOverFont,
                GAME_OVER_STRING,
                gameOverPosition,
                Color.White,
                0f,
                Vector2.Zero,
                GAME_OVER_FONT_SCALE,
                SpriteEffects.None,
                0f
            );
        }

        #endregion UIDrawMethods

        protected override void Update(GameTime gameTime)
        {
            // Process Input
            _inputController.ProcessInput();

            // Update GameObjects
            if (_uiState == UiState.GamePlay && !_isGameOver)
                _snake.Update(gameTime);

            _snakeHeadRectangle.X = (int)Math.Round(_snake.SnakeHead.Position.X - _snake.SnakeHead.Sprite.Width / 2f * _snake.SnakeHead.Sprite.Scale.X);
            _snakeHeadRectangle.Y = (int)Math.Round(_snake.SnakeHead.Position.Y - _snake.SnakeHead.Sprite.Height / 2f * _snake.SnakeHead.Sprite.Scale.Y);

            // Hit edge of play area
            if (!_isGameOver && !_gameAreaRectangle.Contains(_snake.SnakeHead.Position))
            {
                // GAME OVER!
                EndGameAndRecordScore();
            }

            // Hit self
            if (!_isGameOver && _snake.SnakeSegments.Any(s =>
                s.Position.X == _snake.SnakeHead.Position.X && s.Position.Y == _snake.SnakeHead.Position.Y))
            {
                EndGameAndRecordScore();
            }

            if (!_isGameOver && _snakeHeadRectangle.Intersects(_appleRectangle))
            {
                _appleEaten = true;
                _applePlaced = false;
                _eatSoundEffect.Play(1f, 0f, 0f);
                _snake.AddSegment();
            }

            GenerateGrid();

            if (_appleEaten)
            {
                GenerateApple();
            }

            if (_uiState == UiState.StartScreen)
            {
                _startScreenUiFrame.Update(gameTime);
            }

            if (_uiState == UiState.HighScoresScreen)
            {
                _highScoresUiFrame.Update(gameTime);
            }

            _snakeHeadAnimatedSprite.Update(gameTime);

            _startScreenHighScoresToggleButton.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _frameCounter.Update(deltaTime);

            var fps = $"FPS: {_frameCounter.CurrentFramesPerSecond}";

            if (_isGameOver)
            {
                _spriteBatch.Begin();
                DrawGameOverText();
                _spriteBatch.End();
            }

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            if (_showFpsMonitor)
                _spriteBatch.DrawString
                    (
                        _scoreBoardFont,
                        fps,
                        new Vector2(20, 10),
                        Color.Blue,
                        0f,
                        Vector2.Zero,
                        new Vector2(2, 2),
                        SpriteEffects.None,
                    0f
                    );

            if (_isGameOver)
                DrawGameOverText();

            if (_uiState == UiState.GamePlay && !_isGameOver)
            {
                // Draw Game Area
                foreach (Vector2 outlinePixel in _gameAreaRectangle.OutlinePixels())
                {
                    _spriteBatch.Draw(_gameAreaRectangleTexture, outlinePixel, Color.Green);
                }

                // Draw Snake
                _snake.Draw(_spriteBatch, gameTime);

                // Draw Apple
                _appleGameObject.Draw(_spriteBatch, gameTime);
            }

            if (_drawDiagnosticGrid)
            {
                foreach (Rectangle rectangle in _cells)
                {
                    foreach (Vector2 outlinePixel in rectangle.OutlinePixels())
                    {
                        _spriteBatch.Draw(_snakeHeadRectangleTexture, outlinePixel, Color.Blue);
                    }
                }

                // Rectangle around Snake Head
                foreach (Vector2 outlinePixel in _snakeHeadRectangle.OutlinePixels())
                {
                    _spriteBatch.Draw(_snakeHeadRectangleTexture, outlinePixel, Color.Red);
                }

                // Rectangle around Apple
                foreach (Vector2 outlinePixel in _appleRectangle.OutlinePixels())
                {
                    _spriteBatch.Draw(_snakeHeadRectangleTexture, outlinePixel, Color.Red);
                }
            }

            if (_uiState == UiState.StartScreen)
            {
                DrawStartScreenUiFrame(gameTime);
                DrawLogoText();
            }

            if (_uiState == UiState.HighScoresScreen)
            {
                DrawHighScoresUiFrame(gameTime);
                DrawLeaderboardText();
                DrawHighScores(gameTime);
            }

            _startScreenHighScoresToggleButton.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
