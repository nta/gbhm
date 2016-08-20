using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GBH
{
    public static class ClientInput
    {
        private const int UCMD_BACKUP = 64;
        private static int _commandNum;
        private static Queue<UserCommand> _userCommands = new Queue<UserCommand>();
        private static UserCommand[] _userCommandBackups = new UserCommand[UCMD_BACKUP];
        private static bool _leftKey;
        private static bool _rightKey;
        private static bool _upKey;
        private static bool _downKey;
        private static bool _shootKey;
        private static float _vibrationAmount = 0.4f;

        public static void CreateCommand()
        {
            var command = new UserCommand();
            command.ServerTime = (uint)(Game.Time + Client.TimeBase);

            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

            // todo: deadzones?
            if (_upKey || gamePad.ThumbSticks.Left.Y >= 0.05f)
            {
                command.Buttons |= ClientButtons.Forward;
            }

            if (_downKey || gamePad.ThumbSticks.Left.Y <= -0.05f)
            {
                command.Buttons |= ClientButtons.Backward;
            }

            if (_leftKey || gamePad.ThumbSticks.Right.X <= -0.05f)
            {
                command.Buttons |= ClientButtons.RotateLeft;
            }

            if (_rightKey || gamePad.ThumbSticks.Right.X >= 0.05f)
            {
                command.Buttons |= ClientButtons.RotateRight;
            }

            if (_shootKey || gamePad.Buttons.RightShoulder == ButtonState.Pressed)
            {
                command.Buttons |= ClientButtons.Shoot;
                // vibrate controller when shooty
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    _vibrationAmount = MathHelper.Clamp(_vibrationAmount + 0.03f, 0.0f, 1.0f);
                    GamePad.SetVibration(PlayerIndex.One, _vibrationAmount, _vibrationAmount);
                }
            }

            _userCommands.Enqueue(command);

            _commandNum++;
            _userCommandBackups[_commandNum & (UCMD_BACKUP - 1)] = command;

            // stop vibration
            GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
        }

        public static int CommandID
        {
            get
            {
                return _commandNum;
            }
        }

        public static UserCommand? GetCommand()
        {
            if (_userCommands.Count == 0)
            {
                return null;
            }

            return _userCommands.Dequeue();
        }

        public static UserCommand? GetCommand(int index)
        {
            if (index > _commandNum)
            {
                throw new ArgumentException("command index too high");
            }

            if (index <= (_commandNum - UCMD_BACKUP))
            {
                return null;
            }

            return _userCommandBackups[index & (UCMD_BACKUP - 1)];
        }

        public static int NumCommands
        {
            get
            {
                return _userCommands.Count;
            }
        }

        public static void HandleKey(Keys key, bool down)
        {
            if (key == Keys.Left)
            {
                _leftKey = down;
            }

            if (key == Keys.Right)
            {
                _rightKey = down;
            }

            if (key == Keys.Up)
            {
                _upKey = down;
            }

            if (key == Keys.Down)
            {
                _downKey = down;
            }

            if (key == Keys.Space)
            {
                _shootKey = down;
            }
        }
    }
}
