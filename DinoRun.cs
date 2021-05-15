using DinoRun.Entities;
using DinoRun.Graphics;
using DinoRun.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DinoRun
{
    public class DinoRun : Game
    {
        private const string SpritesheetAsset = "DinoSpritesheet";
        private const string ButtonPressAsset = "ButtonPress";
        private const string HitAsset = "Hit";
        private const string ScoreReachedAsset = "ScoreReached";

        public const int GAME_WIDTH = 600;
        public const int GAME_HEIGHT = 150;

        public const int DINO_START_X = 1;
        public const int DINO_START_Y = GAME_HEIGHT - 16;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SoundEffect _hitSFX;
        private SoundEffect _buttonPressSFX;
        private SoundEffect _scoreReachedSFX;

        private Texture2D _spritesheetTexture;

        private Dino _playerDino;

        private InputController _inputController;

        public DinoRun()
        {        
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            _graphics.PreferredBackBufferWidth = GAME_WIDTH;
            _graphics.PreferredBackBufferHeight = GAME_HEIGHT;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _hitSFX = Content.Load<SoundEffect>(HitAsset);
            _buttonPressSFX = Content.Load<SoundEffect>(ButtonPressAsset);
            _scoreReachedSFX = Content.Load<SoundEffect>(ScoreReachedAsset);

            _spritesheetTexture = Content.Load<Texture2D>(SpritesheetAsset);

            _playerDino = new Dino(_spritesheetTexture, new Vector2(DINO_START_X, DINO_START_Y - Dino.DINO_SPRITE_HEIGHT), _buttonPressSFX);
            _inputController = new InputController(_playerDino);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);

            _inputController.ProcessControls(gameTime);
            
            _playerDino.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();

            _playerDino.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
