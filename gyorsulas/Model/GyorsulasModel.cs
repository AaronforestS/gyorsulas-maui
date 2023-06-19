using gyorsulas.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace gyorsulas.Model
{
    public class GyorsulasModel
    {
        public event EventHandler<GyorsulasArgs>? OnGameEnd;
        public event EventHandler<GyorsulasArgs>? OnMapUpdate;

        private GyorsulasData persistence;
        private GyorsulasFileAccessInterface fileAccess;
        private bool gameEnd;
        private bool gamePaused;


        public GyorsulasModel(GyorsulasFileAccessInterface _fileAccess)
        {
            persistence = new GyorsulasData();
            fileAccess = _fileAccess;
            gameEnd = false;
            gamePaused = false;
        }

        #region Public methods
        public void TimerTick(int col)
        {
            persistence._fuel -= 1;
            persistence._gameTime++;

            IsGameEnded();
            if (gameEnd)
                return;

            if (gamePaused)
                return;

            int speedUp = 30;
            if (persistence._gameTime % 3 == 0)
            {
                persistence.SetGrid(0, col, 2);
            }
            else if (persistence._gameTime % 5 == 0)
            {
                if (persistence._tickRate - speedUp <= 0)
                {
                    persistence._tickRate = 1;
                }
                else
                {
                    persistence._tickRate -= speedUp;
                }
            }
            UpdateMap();
        }
        public void ProgressGame()
        {
            if (gamePaused || gameEnd)
                return;

            if (persistence.IsThePlayerUnderAFuel())
            {
                persistence.RefillFuel();
                RemoveFuelNearThePlayer("top");
            }

            MoveAllFuel();
            UpdateMap();
        }
        public void Move(bool isLeft)
        {
            if (gameEnd || gamePaused)
                return;

            switch (IsTheDesiredSpotBlocked(isLeft))
            {
                case -1:
                    persistence.RefillFuel();
                    RemoveFuelNearThePlayer("left"); 
                    break;
                case 1:
                    persistence.RefillFuel();
                    RemoveFuelNearThePlayer("right"); 
                    break;
                default: 
                    break;
            }
            if (isLeft)
            {
                persistence.MoveToTheLeft();   
            }
            else
            {
                persistence.MoveToTheRight();
            }
            UpdateMap();
        }
        public void Pause()
        {
            gamePaused = !gamePaused;
            UpdateMap();
        }
        public void NewGame()
        {
            persistence = new GyorsulasData();
            gamePaused = false;
            gameEnd = false;
            UpdateMap();
        }
        public async Task LoadGyorsulasGame(string path)
        {
            if (persistence == null)
                throw new InvalidDataException();

            persistence = await fileAccess.GyorsulasLoad(path);
            UpdateMap();
        }
        public async Task SaveGyorsulasGame(string path)
        {
            if (persistence == null)
                throw new InvalidDataException();

            await fileAccess.GyorsulasSave(path,persistence);
            UpdateMap();
        }
        public void UpdateMap()
        {
            OnMapUpdate?.Invoke(this, new GyorsulasArgs(persistence._y, persistence._x, persistence._gameTime, persistence._playerPosition, persistence._tickRate, persistence._fuel, gamePaused, gameEnd));
        }
        #endregion
        #region Private methods
        private int IsTheDesiredSpotBlocked(bool isLeft)
        {
            int i = persistence._playerPosition;
            int x = persistence._x;
            int y = persistence._y;

            if (i - 1 >= 0 && persistence[y - 1, i - 1] == 2 && isLeft)
            {
                return -1;
            }
            else if (i + 1 < x && persistence[y - 1, i + 1] == 2 && !isLeft)
            {
                return 1;
            }
            return 0;
        }
        private void RemoveFuelNearThePlayer(string where)
        {
            int y = persistence._y;
            int x = persistence._playerPosition;

            switch (where)
            {
                case "top": persistence.SetGrid(y - 2, x, 0); break;
                case "left": persistence.SetGrid(y - 1, x - 1, 0); break;
                case "right": persistence.SetGrid(y - 1, x + 1, 0); break;
                default: throw new Exception();
            }
        }
        private void MoveAllFuel()
        {
            int x = persistence._x;
            int y = persistence._y;
            for (int i = y - 1; i >= 0; i--)
            {
                for (int j = x - 1; j >= 0; j--)
                {
                    if (persistence[i, j] == 2)
                    {
                        persistence.SetGrid(i, j, 0);
                        if (i != y - 1)
                        {
                            persistence.SetGrid(i + 1, j, 2);
                        }
                    }
                }
            }
        }
        private void IsGameEnded()
        {
            if (persistence._fuel <= 0)
            {
                gameEnd = true;
                OnGameEnd?.Invoke(this, new GyorsulasArgs(persistence._fuel, persistence._tickRate, persistence._gameTime, gameEnd));
            }
        }
        #endregion
        #region Properties for game initialisation, map drawing and testing
        public int width { get { return persistence._x; } }
        public int height {get { return persistence._y; } }
        public int[,] GetGrid { get { return persistence._grid; } }
        public int GetTickRate { get { return persistence._tickRate; } }
        public int GetGameTime { get { return persistence._gameTime; } }
        public int GetFuel { get { return persistence._fuel; } }
        public GyorsulasData Persistence { get { return persistence; } }
        public bool GameEnd { get { return gameEnd; } }
        #endregion
    }
}
