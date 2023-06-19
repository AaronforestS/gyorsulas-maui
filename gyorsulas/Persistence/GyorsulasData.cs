using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gyorsulas.Persistence
{
    public class GyorsulasData
    {
        private const int x = 5;
        private const int y = 9;
        public int _y { get { return y; } }
        public int _x { get { return x; } }
        public int _gameTime { get; set; }
        public int _fuel { get; set; }
        public int _playerPosition { get; set; }
        public int _tickRate { get; set; }
        public int[,] _grid { get; set; }

        public GyorsulasData()
        {
            _gameTime = 0;
            _playerPosition = 2;
            _grid = new int[y, x];
            _grid[y - 1, _playerPosition] = 1;
            _tickRate = 1000;
            _fuel = 100;
        }
        public GyorsulasData(int gameTime, int fuel, int playerPosition, int tickRate, int[,] grid)
        {
            _gameTime = gameTime;
            _fuel = fuel;
            _playerPosition = playerPosition;
            _tickRate = tickRate;
            _grid = grid;
        }

        public void MoveToTheLeft()
        {
            if (_playerPosition == 0)
            {
                return;
            }
            else
            {
                _grid[y - 1, _playerPosition] = 0;
                _playerPosition--;
                _grid[y - 1, _playerPosition] = 1;
            }
        }
        public void MoveToTheRight()
        {
            if (_playerPosition == _x-1)
            {
                return;
            }
            else
            {
                _grid[y - 1, _playerPosition] = 0;
                _playerPosition++;
                _grid[y - 1, _playerPosition] = 1;
            }
        }
        public bool IsThePlayerUnderAFuel()
        {
            if (_grid[y-2, _playerPosition] == 2)
            {
                return true;
            }
            return false;
        }
        public void RefillFuel()
        {
            if(_fuel < 85)
            {
                _fuel += 15;
            }
            else
            {
                _fuel = 100;
            }
        }
        public void SetGrid(int i, int j, int value)
        {
            if (value <= 2 && value >= 0 && i >= 0 && i < y && j >= 0 && j < x)
            {
                _grid[i, j] = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
        public int this[int y, int x] { get { return GetGrid(y ,x); } }
        public int GetGrid(int i, int j)
        {
            if(i >= 0 && i < y && j >= 0 && j < x)
            return _grid[i, j];

            throw new ArgumentOutOfRangeException();
        }
    }
}
