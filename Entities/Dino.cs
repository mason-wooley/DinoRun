using DinoRun.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinoRun.Entities
{
    public class Dino : IGameEntity
    {
        private const float RUN_ANIMATION_FRAME_DURATION = 0.1f;
        private const float MIN_JUMP_HEIGHT = 60f;
        // pixels per second
        private const float GRAVITY = 1600f;
        private const float JUMP_START_VELOCITY = -480f;

        private const float CANCEL_JUMP_VELOCITY = -100f;

        private const float DINO_DROP_VELOCITY = 600f;

        private const int IDLE_BACKGROUND_SPRITE_Y = 0;
        private const int IDLE_BACKGROUND_SPRITE_X = 40;

        public const int DINO_SPRITE_X = 848;
        public const int DINO_SPRITE_Y = 0;
        public const int DINO_SPRITE_WIDTH = 44;
        public const int DINO_SPRITE_HEIGHT = 52;

        private const float DINO_BLINK_MIN_TIME = 2f;
        private const float DINO_BLINK_MAX_TIME = 10f;
        private const float BLINK_ANIMATION_DURATION = 0.2f;

        private const int DINO_RUNNING_SPRITE_ONE_X = DINO_SPRITE_X + 2 * DINO_SPRITE_WIDTH;
        private const int DINO_RUNNING_SPRITE_ONE_Y = 0;

        private const int DINO_RUNNING_SPRITE_TWO_X = DINO_SPRITE_X + 3 * DINO_SPRITE_WIDTH;
        private const int DINO_RUNNING_SPRITE_TWO_Y = 0;

        private const int DINO_DUCKING_SPRITE_ONE_X = DINO_SPRITE_X  + 6 * DINO_SPRITE_WIDTH;
        private const int DINO_DUCKING_SPRITE_ONE_Y = 0;
        private const int DINO_DUCKING_SPRITE_WIDTH = 59;
        private Sprite _idleDinoBackgroundSprite;
        private Sprite _idleSprite;
        private Sprite _idleBlinkSprite;

        private float _verticalVelocity;
        private float _dropVelocity;
        private float _startPosY;

        private SoundEffect _jumpSound;

        private SpriteAnimation _blinkAnimation;
        private SpriteAnimation _runAnimation;
        private SpriteAnimation _duckAnimation;

        private Random _random;

        public DinoState State { get; private set; }

        public Vector2 Position { get; set; }

        public bool IsAlive { get; private set; }
        
        public float Speed { get; private set; }

        public int DrawOrder { get; set; }

        public Dino(Texture2D spritesheet, Vector2 position, SoundEffect jumpSound)
        {
            Position = position;
            
            _startPosY = Position.Y;

            _idleDinoBackgroundSprite = new Sprite(spritesheet, IDLE_BACKGROUND_SPRITE_X, IDLE_BACKGROUND_SPRITE_Y, DINO_SPRITE_WIDTH, DINO_SPRITE_HEIGHT);
            State = DinoState.Idle;

            _jumpSound = jumpSound;

            _random = new Random();

            _idleSprite = new Sprite(spritesheet, DINO_SPRITE_X, DINO_SPRITE_Y, DINO_SPRITE_WIDTH, DINO_SPRITE_HEIGHT);
            _idleBlinkSprite = new Sprite(spritesheet, DINO_SPRITE_X + DINO_SPRITE_WIDTH, DINO_SPRITE_Y, DINO_SPRITE_WIDTH, DINO_SPRITE_HEIGHT);

            _blinkAnimation = new SpriteAnimation();
            CreateBlinkAnimation();
            _blinkAnimation.Play();

            _runAnimation = new SpriteAnimation();
            _runAnimation.AddFrame(new Sprite(spritesheet, DINO_RUNNING_SPRITE_ONE_X, DINO_RUNNING_SPRITE_ONE_Y, DINO_SPRITE_WIDTH, DINO_SPRITE_HEIGHT), 0);
            _runAnimation.AddFrame(new Sprite(spritesheet, DINO_RUNNING_SPRITE_TWO_X, DINO_RUNNING_SPRITE_TWO_Y, DINO_SPRITE_WIDTH, DINO_SPRITE_HEIGHT), RUN_ANIMATION_FRAME_DURATION);
            _runAnimation.AddFrame(_runAnimation[0].Sprite, RUN_ANIMATION_FRAME_DURATION * 2);
            _runAnimation.Play();

            _duckAnimation = new SpriteAnimation();
            _duckAnimation.AddFrame(new Sprite(spritesheet, DINO_DUCKING_SPRITE_ONE_X, DINO_DUCKING_SPRITE_ONE_Y, DINO_DUCKING_SPRITE_WIDTH, DINO_SPRITE_HEIGHT), 0);
            _duckAnimation.AddFrame(new Sprite(spritesheet, DINO_DUCKING_SPRITE_ONE_X + DINO_DUCKING_SPRITE_WIDTH, DINO_DUCKING_SPRITE_ONE_Y, DINO_DUCKING_SPRITE_WIDTH, DINO_SPRITE_HEIGHT), RUN_ANIMATION_FRAME_DURATION);
            _duckAnimation.AddFrame(_duckAnimation[0].Sprite, RUN_ANIMATION_FRAME_DURATION * 2);
            _duckAnimation.Play();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (State == DinoState.Idle)
            {
                _idleDinoBackgroundSprite.Draw(spriteBatch, Position);
                _blinkAnimation.Draw(spriteBatch, Position);
            }
            else if (State == DinoState.Jumping || State == DinoState.Falling)
            {
                _idleSprite.Draw(spriteBatch, Position);
            }
            else if (State == DinoState.Running)
            {
                _runAnimation.Draw(spriteBatch, Position);
            }
            else if (State == DinoState.Ducking)
            {
                _duckAnimation.Draw(spriteBatch, Position);
            }
        }

        public void Update(GameTime gameTime)
        {

            if (State == DinoState.Idle)
            {
                if (!_blinkAnimation.IsPlaying)
                {
                    CreateBlinkAnimation();
                    _blinkAnimation.Play();
                }

                _blinkAnimation.Update(gameTime);
            }
            else if (State == DinoState.Jumping || State == DinoState.Falling)
            {
                Position = new Vector2(Position.X, Position.Y + _verticalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds + _dropVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
                _verticalVelocity += GRAVITY * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_verticalVelocity >= 0)
                    State = DinoState.Falling;

                if (Position.Y >= _startPosY)
                {
                    Position = new Vector2(Position.X, _startPosY);
                    _verticalVelocity = 0;
                    State = DinoState.Running;
                }
            }
            else if (State == DinoState.Running)
            {
                _runAnimation.Update(gameTime);
            }
            else if (State == DinoState.Ducking)
            {
                _duckAnimation.Update(gameTime);
            }

            _dropVelocity = 0;
        }
        
        private void CreateBlinkAnimation()
        {
            _blinkAnimation.Clear();
            _blinkAnimation.ShouldLoop = false;

            double blinkTimeStamp = DINO_BLINK_MIN_TIME + _random.NextDouble() * (DINO_BLINK_MAX_TIME - DINO_BLINK_MIN_TIME);

            _blinkAnimation.AddFrame(_idleSprite, 0);
            _blinkAnimation.AddFrame(_idleBlinkSprite, (float)blinkTimeStamp);
            _blinkAnimation.AddFrame(_idleSprite, (float)blinkTimeStamp + BLINK_ANIMATION_DURATION);
        }

        public bool BeginJump()
        {
            if (State == DinoState.Jumping || State == DinoState.Falling)
                return false;

            _jumpSound.Play();

            State = DinoState.Jumping;

            _verticalVelocity = JUMP_START_VELOCITY;

            return true;
        }

        public bool CancelJump()
        {
            if (State != DinoState.Jumping || (_startPosY - Position.Y) < MIN_JUMP_HEIGHT)
                return false;

            _verticalVelocity = _verticalVelocity < CANCEL_JUMP_VELOCITY ? CANCEL_JUMP_VELOCITY : 0;

            return true;
        }

        public bool Duck()
        {
            if (State == DinoState.Jumping || State == DinoState.Falling)
                return false;

            State = DinoState.Ducking;

            return true;
        }

        public bool GetUp()
        {
            if (State != DinoState.Ducking)
                return false;

            State = DinoState.Running;

            return true;
        }

        public bool Drop()
        {
            if (State != DinoState.Falling && State != DinoState.Jumping)
                return false;

            State = DinoState.Falling;
            
            _dropVelocity = DINO_DROP_VELOCITY;

            return true;
        }
    }
}
