using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using gyorsulas.Model;
using gyorsulas.Persistence;
using Microsoft.Win32;
using System.IO;

namespace gyorsulas_wpf.ViewModel
{
    public class GyorsulasViewModel : ViewModelBase
    {
        private GyorsulasModel _model;
        private Random r;
        private System.Timers.Timer timer;
        private System.Timers.Timer tickRate;

        public event EventHandler Saving;
        public event EventHandler Loading;
        public event EventHandler<GyorsulasArgs> GameEnd;

        public ObservableCollection<GyorsulasField> Fields { get; set; }

        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand PauseGameCommand { get; private set; }
        public DelegateCommand MoveLeftCommand { get; private set; }
        public DelegateCommand MoveRightCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public RowDefinitionCollection Rows
        {
            get { return rows; } private set { rows = value; OnPropertyChanged(); }
        }
        private RowDefinitionCollection rows;
        public ColumnDefinitionCollection Cols
        {
            get { return cols; } private set { cols = value; OnPropertyChanged(); }
        }
        private ColumnDefinitionCollection cols;
        
        private int _gameTime;
        private int _fuel;
        private string pauseButtonText = "Pause";
        private bool isEnabled = false;
        public int gameTime { get { return _gameTime; } set { _gameTime = value; OnPropertyChanged(); } }
        public int fuel { get { return _fuel; } set { _fuel = value; OnPropertyChanged(); } }
        public string PauseButtonText { get { return pauseButtonText; } private set { pauseButtonText = value; OnPropertyChanged(); } }
        public bool IsEnabled { get { return isEnabled; } private set { isEnabled = value; OnPropertyChanged(); } }

        public GyorsulasViewModel(GyorsulasModel model)
        {
            Fields = new ObservableCollection<GyorsulasField>();
            _model = model;
            r = new Random(System.DateTime.Now.Millisecond);

            Rows = new RowDefinitionCollection(Enumerable.Repeat(new RowDefinition(GridLength.Star), _model.height).ToArray());
            Cols = new ColumnDefinitionCollection(Enumerable.Repeat(new ColumnDefinition(GridLength.Star), _model.width).ToArray());

            _model.OnMapUpdate += _model_OnMapUpdate;
            _model.OnGameEnd += _model_OnGameEnd;

            NewGameCommand = new DelegateCommand(param => _model.NewGame());
            PauseGameCommand = new DelegateCommand(param => _model.Pause());
            MoveLeftCommand = new DelegateCommand(param => _model.Move(true));
            MoveRightCommand = new DelegateCommand(param => _model.Move(false));
            SaveGameCommand = new DelegateCommand(param => TryToSaveGyorsulasGame());
            LoadGameCommand = new DelegateCommand(param => TryToLoadGyorsulasGame());

            tickRate = new System.Timers.Timer();
            tickRate.Interval = model.GetTickRate;
            tickRate.Elapsed += TickRate_Elapsed;

            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;

            GenerateMap();
            _model.NewGame();
            UpdateFields(false);

            timer.Start();
            tickRate.Start();
        }

        private void _model_OnGameEnd(object sender, GyorsulasArgs e)
        {
            timer.Stop();
            tickRate.Stop();

            GameEnd?.Invoke(this, e);
        }

        private void TryToLoadGyorsulasGame()
        {
            Loading?.Invoke(this, EventArgs.Empty);
        }

        private void TryToSaveGyorsulasGame()
        {
            Saving?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateFields(bool isPaused)
        {
            gameTime = _model.GetGameTime;
            fuel = _model.GetFuel;

            if (isPaused)
            {
                PauseButtonText = "Continue";
            }
            else
            {
                PauseButtonText = "Pause";
            }

            foreach(var field in Fields)
            {
                int i = field.X;
                int j = field.Y;

                if (_model.GetGrid[i,j] == 0)
                {
                    field.BGColor = 0;
                }
                else if (_model.GetGrid[i, j] == 1)
                {
                    field.BGColor = 1;
                }
                else if (_model.GetGrid[i, j] == 2)
                {
                    field.BGColor = 2;
                }

            }
        }
        private void GenerateMap()
        {
            int width = _model.width;
            int height = _model.height;

            for (int i = 1; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Fields.Add(new GyorsulasField(i, j));
                }
            }
        }

        private void _model_OnMapUpdate(object? sender, GyorsulasArgs e)
        {
            if(!e._GamePaused && !e._GameEnd)
            {
                timer.Start();
                tickRate.Start();
            }
            if(!e._GameEnd && e._GamePaused)
            {
                IsEnabled = true;
            }
            else
            {
                IsEnabled= false;
            }

            UpdateFields(e._GamePaused);
        }

        private void TickRate_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _model.ProgressGame();
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _model.TimerTick(r.Next(0, 4));
            tickRate.Interval = _model.GetTickRate;
        }
    }
}
