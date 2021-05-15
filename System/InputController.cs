using DinoRun.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinoRun.System
{
    public class InputController
    {
        private Dino _playerDino;

        private KeyboardState _previousKeyboardState;

        public InputController(Dino playerDino)
        {
            _playerDino = playerDino;
        }

        public void ProcessControls(GameTime gametime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            bool isJumpKeyPressed = keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Space);
            bool wasJumpKeyPressed = _previousKeyboardState.IsKeyDown(Keys.Up) || _previousKeyboardState.IsKeyDown(Keys.Space);

            if (!wasJumpKeyPressed && isJumpKeyPressed)
            {
                if (_playerDino.State != DinoState.Jumping)
                    _playerDino.BeginJump();
                else
                    _playerDino.CancelJump();
            }
            else if (_playerDino.State == DinoState.Jumping && !isJumpKeyPressed)
            {
                _playerDino.CancelJump();
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                if (_playerDino.State == DinoState.Jumping || _playerDino.State == DinoState.Falling)
                    _playerDino.Drop();
                else
                    _playerDino.Duck();
            }
            else if (_playerDino.State == DinoState.Ducking && !keyboardState.IsKeyDown(Keys.Down))
            {
                _playerDino.GetUp();
            }
            _previousKeyboardState = keyboardState;
        }
    }
}
