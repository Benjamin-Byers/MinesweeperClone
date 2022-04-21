using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for GameController.xaml
    /// </summary>
    public partial class GameController : UserControl
    {
        private const int MAX_WIDTH = 800;
        private const int MAX_HEIGHT = 800;

        private int displayedTime = 0;
        private int mines = 40;
        private int xTiles = 16;
        private int yTiles = 16;
        private int tileLength;

        private Stopwatch stopwatch;
        private DispatcherTimer timer;

        private GameState gameState = GameState.Invalid;
        private GameLevel level = GameLevel.Intermediate;

        private IGameLogic game;

        public GameController()
        {
            InitializeComponent();

            stopwatch = new Stopwatch();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;

            timeView.InitializeDisplay(150, 3, true);
            mineCount.InitializeDisplay(150, 3, true);

            timeView.UpdateDisplay(0);
            mineCount.UpdateDisplay(0);

            game = IGameLogicFactory.Create();

            MouseLeftButtonUp += GameController_MouseLeftButtonUp;
            MouseRightButtonUp += GameController_MouseRightButtonUp;
            KeyDown += GameController_KeyDown;

            NewGame();
        }

        public void ToggleQuestionMarks()
        {
            game.ToggleQuestionMarks();
            gameGrid.UpdateGrid(game.GetTileStates());
        }

        public void ChangeLevel(GameLevel level)
        {
            this.level = level;

            switch (level)
            {
                case GameLevel.Beginner:
                    mines = 10;
                    xTiles = 9;
                    yTiles = 9;
                    break;
                case GameLevel.Intermediate:
                    mines = 40;
                    xTiles = 16;
                    yTiles = 16;
                    break;
                case GameLevel.Advanced:
                    mines = 99;
                    xTiles = 30;
                    yTiles = 16;
                    break;
                case GameLevel.Custom:
                    break;
            }
        }

        private void GameController_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2:
                    NewGame();
                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            int time = (int) stopwatch.ElapsedMilliseconds / 1000;
            if (time <= displayedTime) return;

            displayedTime = time;
            timeView.UpdateDisplay(time);
        }


        private void GameController_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point mouseCoords = e.GetPosition(gameGrid);
            int x = (int)(mouseCoords.X - gameGrid.borderMargin)/ tileLength;
            int y = (int)(mouseCoords.Y - gameGrid.borderMargin)/ tileLength;

            if (x >= xTiles || x < 0 || y >= yTiles || y < 0) return;
            gameGrid.UpdateGrid(game.SendMove(x, y, MoveType.Flag));
            mineCount.UpdateDisplay(game.GetRemainingMines());
        }

        private void GameController_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point mouseCoords = e.GetPosition(gameGrid);
            int x = (int)(mouseCoords.X - gameGrid.borderMargin)/ tileLength;
            int y = (int)(mouseCoords.Y - gameGrid.borderMargin)/ tileLength;

            if (x >= xTiles || x < 0 || y >= yTiles || y < 0) return;
            gameGrid.UpdateGrid(game.SendMove(x, y, MoveType.Open));
            gameState = game.GetGameState();

            if (gameState == GameState.Playing)
            {
                if (!timer.IsEnabled)
                {
                    stopwatch.Restart();
                    timer.Start();
                }
            }
            else if (gameState == GameState.Win || gameState == GameState.Lose)
            {
                mineCount.UpdateDisplay(0);
                stopwatch.Stop();
                timer.Stop();
            }
        }

        public void NewGame()
        {
            if (level == GameLevel.Custom)
            {
                game.NewGame(xTiles, yTiles, mines);
            }
            else
            {
                game.NewGame(level);
            }

            stopwatch.Reset();
            timer.Stop();
            displayedTime = 0;
            timeView.UpdateDisplay(0);
            mineCount.UpdateDisplay(mines);
            gameGrid.InitializeTiles(MAX_WIDTH, MAX_HEIGHT, xTiles, yTiles);
            this.tileLength = gameGrid.tileLength;
        }

        private void newGameButton_Click(object sender, RoutedEventArgs e) => NewGame();
    }
}
