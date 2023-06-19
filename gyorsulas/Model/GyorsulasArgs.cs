using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gyorsulas.Model
{
    public class GyorsulasArgs : EventArgs
    {
        private int _y;
        private int _x;
        private int _playerPosition;
        private int _gameTime;
        private int _fuel;
        private int _tickRate;
        private bool _gamePaused;
        private bool _gameEnd;

        public int _Y { get { return _y; } }
        public int _X { get { return _x; } }
        public int _PlayerPosition { get { return _playerPosition; } }
        public int _GameTime { get { return _gameTime; } }
        public int _Fuel { get { return _fuel;  } }
        public int _TickRate { get { return _tickRate; } }
        public bool _GamePaused { get { return _gamePaused; } }
        public bool _GameEnd { get { return _gameEnd; } }
        

        public GyorsulasArgs(int y, int x, int gameTime, int playerPosition, int tickRate, int fuel, bool gamePaused, bool gameEnd)
        {
            _y = y;
            _x = x;
            _gameTime = gameTime;
            _playerPosition = playerPosition;
            _tickRate = tickRate;
            _fuel = fuel;
            _gamePaused = gamePaused;
            _gameEnd = gameEnd;
        }
        public GyorsulasArgs(int fuel,int tickRate,int gameTime,bool gameEnd)
        {
            _fuel = fuel;
            _tickRate = tickRate;
            _gameTime = gameTime;
            _gameEnd = gameEnd;
        }
        public GyorsulasArgs(bool gamePaused, bool gameEnd)
        {
            _gamePaused = gamePaused;
            _gameEnd = gameEnd;
        }
    }
}