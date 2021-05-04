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
        private const int GAME_AREA_MARGIN_LEFT = 20;
        private const int GAME_AREA_MARGIN_TOP = 90;
        private const int GAME_AREA_PADDING = 10;
        private const string UI_BLUE_SPRITE_SHEET_NAME = "UiBlueSheet";
        private const string UI_BLUE_SPRITE_SHEET_HOVER_NAME = "UiBlueSheetHover";
        #endregion Constants

        #region Events

        public event Action<SnakeGame, UIState> UIStateChangeEvent;

        #endregion Events


        private const float ROTATE90_CW = (float)(90 * Math.PI / 180);

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
            private FrameCounter _frameCounter;
        #endregion System

        #region UI
            private UIState _uiState;
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
            private SpriteFont _dialogTitleFont;
            private SpriteFont _dialogPromptFont;
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
            private Texture2D _uiBlueSpriteSheet;
            private Texture2D _uiBlueSpriteSheetHover;
            private Sprite _uiWindowTopLeftSprite;
            private Sprite _uiWindowTopRightSprite;
            private Sprite _uiWindowBottomLeftSprite;
            private Sprite _uiWindowBottomRightSprite;
            private Sprite _uiWindowTopRowSprite;
            private Sprite _uiWindowBottomRowSprite;
            private CenteredUiFrame _confirmHighScoreEntryDialogCenteredUiFrame;
            private CenteredUiFrame _warningHighScoreEntryDialogCenteredUiFrame;
        private Sprite _snakeFrameTopRowRowSprite;
            private Sprite _snakeFrameBottomRowRowSprite;
            private Sprite _snakeFrameRightColumnRowSprite;
            private Sprite _snakeFrameLeftColumnRowSprite;
            private Sprite _uiWindowRightColumnSprite;
            private Sprite _uiWindowLeftColumnSprite;
            private Sprite _uiButtonBlueSheetCancel;
            private Sprite _uiButtonBlueSheetConfirm;
            private CenteredUiDialog _scoreEntryCenteredUiDialog;
            private Sprite _uiButtonBlueSheetHoverCancel;
            private Sprite _uiButtonBlueSheetHoverConfirm;
            private Sprite _uiCheckBoxBlueSheetBox;
            private Sprite _uiCheckBoxBlueSheetChecked;
            private ToggleUiButton _soundEnabledCheckBoxToggle;
            private Sprite _uiCheckBoxBlueSheetHoverChecked;
            private Sprite _uiCheckBoxBlueSheetHoverBox;
            private CenteredUiDialog _scoreEntryWarningCenteredUiDialog;

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
            _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected void OnUIStateChange(SnakeGame snakeGame, UIState uiState)
        {
            Action<SnakeGame, UIState> handler = this.UIStateChangeEvent;
            handler?.Invoke(snakeGame, uiState);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadAssets();

            InitializeGameObjects();

            InitializeDiagnosticObjects();

            GenerateGrid();

            GenerateApple();

            InitializeUiObjects();

            InitializeInputController();

            SetUiState(UIState.StartScreen);

            _scoreBoard.HighScoreEntryCompletedEvent += ScoreBoardOnHighScoreEntryCompletedEvent;
        }

        private void ScoreBoardOnHighScoreEntryCompletedEvent(object sender, EventArgs e)
        {
            _startScreenHighScoresToggleButton.IsEnabled = true;
            SetUiState(UIState.HighScoresScreen);
        }

        private void LoadAssets()
        {
            // Load Fonts
            _logoFont = Content.Load<SpriteFont>("Logo");
            _gameOverFont = Content.Load<SpriteFont>("GameOver");
            _leaderboardFont = Content.Load<SpriteFont>("Arcade");
            _scoreBoardFont = Content.Load<SpriteFont>("ArcadeClassic");
            _dialogTitleFont = Content.Load<SpriteFont>("DialogTitle");
            _dialogPromptFont = Content.Load<SpriteFont>("DialogPrompt");
            // Load Textures
            _appleTexture = Content.Load<Texture2D>(APPLE_SPRITE_SHEET_NAME);
            _snakeHeadSpriteSheet = Content.Load<Texture2D>(SNAKE_HEAD_SPRITE_SHEET_NAME);
            _snakeSegmentsSpriteSheet = Content.Load<Texture2D>(SNAKE_SEGMENTS_SPRITE_SHEET_NAME);
            _uiBlueSpriteSheet = Content.Load<Texture2D>(UI_BLUE_SPRITE_SHEET_NAME);
            _uiBlueSpriteSheetHover = Content.Load<Texture2D>(UI_BLUE_SPRITE_SHEET_HOVER_NAME);
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

            PositionedTexture2D snakeFrameTopRowPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 1, 2);
            _snakeFrameTopRowRowSprite = new Sprite(snakeFrameTopRowPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE, ROTATE90_CW)
            {
                Origin = halfSizeOrigin
            };

            PositionedTexture2D snakeFrameBottomRowPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 1, 2);
            _snakeFrameBottomRowRowSprite = new Sprite(snakeFrameBottomRowPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE, ROTATE90_CW)
            {
                Origin = halfSizeOrigin
            };

            PositionedTexture2D snakeFrameRightColumnPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 1, 2);
            _snakeFrameRightColumnRowSprite = new Sprite(snakeFrameRightColumnPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = halfSizeOrigin
            };

            PositionedTexture2D snakeFrameLeftColumnPositionedTexture2D = new PositionedTexture2D(_snakeSegmentsSpriteSheet, 1, 1, 2);
            _snakeFrameLeftColumnRowSprite = new Sprite(snakeFrameLeftColumnPositionedTexture2D, DEFAULT_SPRITE_SIZE, DEFAULT_SPRITE_SIZE)
            {
                Origin = halfSizeOrigin
            };

            _uiWindowTopRowSprite = new Sprite(_uiBlueSpriteSheet, 143, 10, 10, 10)
            {
                Origin = new Vector2(5, 5)
            };

            _uiWindowBottomRowSprite = new Sprite(_uiBlueSpriteSheet, 182, 10, 10, 10)
            {
                Origin = new Vector2(5, 5)
            };

            _uiWindowRightColumnSprite = new Sprite(_uiBlueSpriteSheet, 153, 180, 10, 10)
            {
                Origin = new Vector2(5, 5)
            };

            _uiWindowLeftColumnSprite = new Sprite(_uiBlueSpriteSheet, 153, 0, 10, 10)
            {
                Origin = new Vector2(5, 5)
            };

            _uiWindowTopLeftSprite = new Sprite(_uiBlueSpriteSheet, 143, 0, 10, 10)
            {
                Origin = new Vector2(5, 5)
            };

            _uiWindowTopRightSprite = new Sprite(_uiBlueSpriteSheet, 143, 180, 10, 10)
            {
                Origin = new Vector2(5, 5)
            };

            _uiWindowBottomRightSprite = new Sprite(_uiBlueSpriteSheet, 182, 180, 10, 10)
            {
                Origin = new Vector2(5, 5)
            };

            _uiWindowBottomLeftSprite = new Sprite(_uiBlueSpriteSheet, 182, 0, 10, 10)
            {
                Origin = new Vector2(5, 5)
            };

            _uiButtonBlueSheetCancel = new Sprite(_uiBlueSpriteSheet, 0, 381, 36, 36);

            _uiButtonBlueSheetConfirm = new Sprite(_uiBlueSpriteSheet, 36, 381, 36, 36);

            _uiButtonBlueSheetHoverCancel = new Sprite(_uiBlueSpriteSheetHover, 0, 381, 36, 36);

            _uiButtonBlueSheetHoverConfirm = new Sprite(_uiBlueSpriteSheetHover, 36, 381, 36, 36);

            _uiCheckBoxBlueSheetBox = new Sprite(_uiBlueSpriteSheet, 194, 288, 49, 49)
            {
                Scale = new Vector2(0.85f, 0.85f)
            };

            _uiCheckBoxBlueSheetChecked = new Sprite(_uiBlueSpriteSheet, 0, 417, 49, 49)
            {
                Scale = new Vector2(0.85f, 0.85f)
            };

            _uiCheckBoxBlueSheetHoverBox = new Sprite(_uiBlueSpriteSheetHover, 194, 288, 49, 49)
            {
                Scale = new Vector2(0.85f, 0.85f)
            };

            _uiCheckBoxBlueSheetHoverChecked = new Sprite(_uiBlueSpriteSheetHover, 0, 417, 49, 49)
            {
                Scale = new Vector2(0.85f, 0.85f)
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

        protected void OnStartScreenHighScoresToggleButtonClick(object sender, EventArgs e)
        {
            if (_uiState != UIState.GamePlay || _isGameOver)
            {
                _startScreenHighScoresToggleButton.IsEnabled = true;

                SetUiState(_startScreenHighScoresToggleButton.IsToggled ? UIState.HighScoresScreen : UIState.StartScreen);
            }
            else
            {
                _startScreenHighScoresToggleButton.IsEnabled = false;
            }
        }

        private void SetUiState(UIState uiState)
        {
            _uiState = uiState;
            OnUIStateChange(this, uiState);
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
            _inputController = new InputController(this, _snake, _scoreBoard);
            _inputController.StartEvent += InputController_StartEvent;
            _inputController.ExitEvent += InputController_ExitEvent;
            _inputController.RestartEvent += InputController_RestartEvent;
            _inputController.HeadTurnEvent += InputController_HeadTurnEvent;
        }

        private void InitializeUiObjects()
        {
            _frameCounter = new FrameCounter(_scoreBoardFont, SCREEN_WIDTH, SCREEN_HEIGHT, Color.Blue, new Vector2(2, 2));
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
                _snakeFrameTopRowRowSprite,
                _snakeFrameBottomRowRowSprite,
                _snakeFrameLeftColumnRowSprite,
                _snakeFrameRightColumnRowSprite,
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
                _snakeFrameTopRowRowSprite,
                _snakeFrameBottomRowRowSprite,
                _snakeFrameLeftColumnRowSprite,
                _snakeFrameRightColumnRowSprite,
                _snakeCwUpToRightCcwLeftToDownSprite,
                _snakeCwRightToDownCcwUpToLeftSprite,
                _snakeCwDownToLeftCcwRightToUpSprite,
                _snakeCwLeftToUpCcwDownToRightSprite,
                Color.FromNonPremultiplied(46, 51, 106, START_SCREEN_TRANSPARENCY)
            );

            _confirmHighScoreEntryDialogCenteredUiFrame = new CenteredUiFrame
            (
                _graphics,
                Vector2.Zero,
                45,
                20,
                SCREEN_WIDTH,
                SCREEN_HEIGHT,
                _uiWindowTopRowSprite,
                _uiWindowBottomRowSprite,
                _uiWindowLeftColumnSprite,
                _uiWindowRightColumnSprite,
                _uiWindowTopLeftSprite,
                _uiWindowTopRightSprite,
                _uiWindowBottomRightSprite,
                _uiWindowBottomLeftSprite,
                new Color(new Vector3(0.9333f, 0.9333f, 0.9333f))
            );

            _warningHighScoreEntryDialogCenteredUiFrame = new CenteredUiFrame
            (
                _graphics,
                Vector2.Zero,
                55,
                20,
                SCREEN_WIDTH,
                SCREEN_HEIGHT,
                _uiWindowTopRowSprite,
                _uiWindowBottomRowSprite,
                _uiWindowLeftColumnSprite,
                _uiWindowRightColumnSprite,
                _uiWindowTopLeftSprite,
                _uiWindowTopRightSprite,
                _uiWindowBottomRightSprite,
                _uiWindowBottomLeftSprite,
                new Color(new Vector3(0.9333f, 0.9333f, 0.9333f))
            );

            UiButton cancelScoreEntryButton = new UiButton(_uiButtonBlueSheetCancel, _uiButtonBlueSheetHoverCancel, Vector2.Zero, 0f);
            UiButton confirmScoreEntryButton = new UiButton(_uiButtonBlueSheetConfirm, _uiButtonBlueSheetHoverConfirm, Vector2.Zero, 0f);
            UiButton confirmWarningButton = new UiButton(_uiButtonBlueSheetConfirm, _uiButtonBlueSheetHoverConfirm, Vector2.Zero, 0f);

            _scoreEntryCenteredUiDialog = new CenteredUiDialog(_graphics.GraphicsDevice, _confirmHighScoreEntryDialogCenteredUiFrame,
                _dialogTitleFont, _dialogPromptFont, "New High Score", "Save High Score Entry?",
                confirmScoreEntryButton, cancelScoreEntryButton, Color.CornflowerBlue, Color.Black)
            {
                HasCancelButton = true
            };

            _scoreEntryWarningCenteredUiDialog = new CenteredUiDialog(_graphics.GraphicsDevice, _warningHighScoreEntryDialogCenteredUiFrame,
                _dialogTitleFont, _dialogPromptFont, "New High Score", "Please Enter Your Name. Don't you want recognition?",
                confirmWarningButton, null, Color.Yellow, Color.Black);

            _scoreBoard = new ScoreBoard(Assembly.GetEntryAssembly().Location, _graphics.GraphicsDevice, _scoreBoardFont, _highScoresUiFrame, _scoreEntryCenteredUiDialog, _scoreEntryWarningCenteredUiDialog, SCREEN_WIDTH, SCREEN_HEIGHT);

            _soundEnabledCheckBoxToggle = new ToggleUiButton(_uiCheckBoxBlueSheetBox, _uiCheckBoxBlueSheetHoverBox, _uiCheckBoxBlueSheetChecked, _uiCheckBoxBlueSheetHoverChecked, new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2), 0f);
            _soundEnabledCheckBoxToggle.IsEnabled = true;
            _soundEnabledCheckBoxToggle.Toggle();
        }

        private void InputController_StartEvent(object sender, EventArgs e)
        {
            InitializeGameObjects();
            InitializeInputController();
            SetUiState(UIState.StartScreen);
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
            SetUiState(UIState.GamePlay);
            GenerateGrid();
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
            if (_uiState == UIState.GamePlay && _moveSoundEffects != null && _soundEnabledCheckBoxToggle.IsToggled)
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
            _isGameOver = true;

            if (_scoreBoard.IsNewHighScore(_snake.Score))
            {
                SetUiState(UIState.HighScoreEntry);
                _scoreBoard.StartHighScoreEntry(_snake.Score);
                _startScreenHighScoresToggleButton.IsEnabled = false;
                if(!_startScreenHighScoresToggleButton.IsToggled)
                    _startScreenHighScoresToggleButton.Toggle();
            }
            else
            {
                _startScreenHighScoresToggleButton.IsEnabled = true;
            }
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

        private void DrawSoundSettingText()
        {
            string soundEffectsStringText = "Sound Effects";
            float soundEffectsScale = 2f;
            Vector2 soundEffectStringSize = _dialogPromptFont.MeasureString(soundEffectsStringText);
            float soundEffectStringX = SCREEN_WIDTH / 2 - soundEffectStringSize.X / 2 - 30;
            float soundEffectStringY = SCREEN_HEIGHT / 2 - soundEffectStringSize.Y / 2 - 130;
            Vector2 soundEffectStringPosition = new Vector2(soundEffectStringX, soundEffectStringY);

            _spriteBatch.DrawString(
                _dialogPromptFont,
                soundEffectsStringText,
                soundEffectStringPosition,
                Color.White,
                0f,
                Vector2.Zero,
                soundEffectsScale,
                SpriteEffects.None,
                0f
            );

            var effectStringX = soundEffectStringX - _soundEnabledCheckBoxToggle.Width - 10;
            var positionY = soundEffectStringY;
            _soundEnabledCheckBoxToggle.Position = new Vector2(effectStringX, positionY);
        }

        #endregion UIDrawMethods

        protected override void Update(GameTime gameTime)
        {
            // Process Input
            _inputController.ProcessInput();

            // Update GameObjects
            if (_uiState == UIState.GamePlay && !_isGameOver)
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

            if (_uiState == UIState.GamePlay && _snakeHeadRectangle.Intersects(_appleRectangle))
            {
                _appleEaten = true;
                _applePlaced = false;
                _snake.AddSegment();

                if (_soundEnabledCheckBoxToggle.IsToggled)
                    _eatSoundEffect.Play(1f, 0f, 0f);
            }

            GenerateGrid();

            if (_appleEaten)
            {
                GenerateApple();
            }

            if (_uiState == UIState.StartScreen)
            {
                _startScreenUiFrame.Update(gameTime);
                _soundEnabledCheckBoxToggle.Update(gameTime);
            }

            if (_uiState == UIState.HighScoresScreen || _uiState == UIState.HighScoreEntry)
            {
                _highScoresUiFrame.Update(gameTime);
                _scoreBoard.Update(gameTime);
            }

            _snakeHeadAnimatedSprite.Update(gameTime);

            _startScreenHighScoresToggleButton.Update(gameTime);
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            if (_showFpsMonitor)
                _frameCounter.Draw(gameTime, _spriteBatch);

            if (_isGameOver && _uiState == UIState.GamePlay)
            {
                DrawGameOverText();
            }

            if (_uiState == UIState.GamePlay && !_isGameOver)
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

            if (_uiState == UIState.StartScreen)
            {
                DrawStartScreenUiFrame(gameTime);
                _soundEnabledCheckBoxToggle.Draw(_spriteBatch, gameTime);
                DrawSoundSettingText();
                DrawLogoText();
            }

            //ToDo: Remove HighScoreEntry condition
            if (_uiState == UIState.HighScoresScreen || _uiState == UIState.HighScoreEntry)
            {
                DrawHighScoresUiFrame(gameTime);
                DrawLeaderboardText();
                _scoreBoard.Draw(_spriteBatch, gameTime);
            }

            _startScreenHighScoresToggleButton.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
