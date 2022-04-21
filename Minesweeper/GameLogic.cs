using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace Minesweeper
{
    class GameLogic : IGameLogic
    {
        public bool questionMarks;
        private int xTiles;
        private int yTiles;
        private int mines;
        private int flagCount;

        private TileState[][] revealedGrid;
        private TileState[][] tileStates;
        private GameState gameState;

        public GameLogic()
        {
            xTiles = 9;
            yTiles = 9;
            mines = 10;

            InitializeTiles();
            gameState = GameState.Ready;
        }

        public TileState[][] GetTileStates()
        {
            return revealedGrid;
        }

        public int GetRemainingMines()
        {
            return mines - flagCount;
        }

        public GameState GetGameState()
        {
            return gameState;
        }

        public void ToggleQuestionMarks()
        {
            if (!questionMarks)
            {
                questionMarks = true;
                return;
            }

            questionMarks = false;

            for (int i = 0; i < revealedGrid.Length; i++)
            {
                for (int j = 0; j < revealedGrid[i].Length; j++)
                {
                    if (revealedGrid[i][j] == TileState.Questioned)
                    {
                        revealedGrid[i][j] = TileState.Closed;
                    }
                }
            }
        }

        public void NewGame(GameLevel level)
        {
            switch(level)
            {
                case GameLevel.Beginner:
                    NewGame(9, 9, 10);
                    break;
                case GameLevel.Intermediate:
                    NewGame(16, 16, 40);
                    break;
                case GameLevel.Advanced:
                    NewGame(30, 16, 99);
                    break;
            }
        }

        public void NewGame(int xTiles, int yTiles, int mines)
        {
            if (xTiles <= 0 || xTiles > 100)
            {
                gameState = GameState.Invalid;
                return;
            }

            if (yTiles <= 0 || yTiles > 100)
            {
                gameState = GameState.Invalid;
                return;
            }

            if (mines <= 0 || mines > xTiles * yTiles)
            {
                gameState = GameState.Invalid;
                return;
            }

            this.xTiles = xTiles;
            this.yTiles = yTiles;
            this.mines = mines;
            this.flagCount = 0;

            InitializeTiles();

            gameState = GameState.Ready;

            return;
        }

        public TileState[][] SendMove(int x, int y, MoveType moveType)
        {
            if (revealedGrid[x][y] != TileState.Closed  && 
                revealedGrid[x][y] != TileState.Flagged && 
                revealedGrid[x][y] != TileState.Questioned) return revealedGrid;

            if (gameState != GameState.Ready && gameState != GameState.Playing) return revealedGrid;

            switch (moveType)
            {
                case MoveType.Open:
                    OpenSquare(x, y);
                    break;
                case MoveType.Flag:
                    FlagSquare(x, y);
                    break;
                case MoveType.Question:
                    QuestionSquare(x, y);
                    break;
            }

            if (gameState != GameState.Lose) CheckWin();
            if (gameState == GameState.Win) RevealFlags();

            return revealedGrid;
        }

        private void CheckWin()
        {
            if (gameState != GameState.Playing) return;

            for (int i = 0; i < xTiles; i++)
            {
                for (int j = 0; j < yTiles; j++)
                {
                    if (tileStates[i][j] == TileState.Mine) continue;
                    if (revealedGrid[i][j] != tileStates[i][j]) return;
                }
            }

            gameState = GameState.Win;
        }

        private void OpenSquare(int x, int y)
        {
            if (revealedGrid[x][y] != TileState.Closed) return;

            if (gameState == GameState.Ready)
            {
                SetMinefield(x, y);
                gameState = GameState.Playing;
            }

            if (tileStates[x][y] == TileState.Mine)
            {
                RevealMines(x, y);
                gameState = GameState.Lose;
                return;
            }
            
            RevealTiles(x, y);
        }

        private void FlagSquare(int x, int y)
        {
            if (revealedGrid[x][y] == TileState.Closed)
            {
                revealedGrid[x][y] = TileState.Flagged;
                flagCount++;
                return;
            }

            if (revealedGrid[x][y] == TileState.Flagged && questionMarks)
            {
                revealedGrid[x][y] = TileState.Questioned;
                flagCount--;
                return;
            }

            if (!questionMarks) flagCount--;

            revealedGrid[x][y] = TileState.Closed;
        }

        private void QuestionSquare(int x, int y)
        {
            if (!questionMarks) return;

            if (revealedGrid[x][y] == TileState.Questioned)
            {
                revealedGrid[x][y] = TileState.Closed;
                return;
            }

            if (revealedGrid[x][y] == TileState.Flagged) flagCount++;

            revealedGrid[x][y] = TileState.Questioned;
        }

        private void InitializeTiles()
        {
            tileStates = new TileState[xTiles][];
            revealedGrid = new TileState[xTiles][];

            for (int i = 0; i < tileStates.Length; i++)
            {
                tileStates[i] = new TileState[yTiles];
                revealedGrid[i] = new TileState[yTiles];

                for (int j = 0; j < tileStates[i].Length; j++)
                {
                    tileStates[i][j] = TileState.Closed;
                    revealedGrid[i][j] = TileState.Closed;
                }
            }
        }

        private void SetMinefield(int tileX, int tileY)
        {
            Random random = new Random();

            Tuple<int, int>[] tiles = new Tuple<int, int>[xTiles * yTiles];

            bool inSquare(int x, int y)
            {
                if (x > tileX + 1 || x < tileX - 1) return false;
                if (y > tileY + 1 || y < tileY - 1) return false;
                return true;
            }

            int index = 0;
            for (int i = 0; i < xTiles; i++)
            {
                for (int j = 0; j < yTiles; j++)
                {
                    if (inSquare(i, j)) continue;
                    tiles[index++] = new Tuple<int, int>(i, j);
                }
            }

            int max = index;

            for (int i = 0; i < mines; i++)
            {
                int r = random.Next(0, index--);
                Tuple<int, int> temp = tiles[index];

                tiles[index] = tiles[r];
                tiles[r] = temp;
            }

            for (; index < max; index++)
            {
                Tuple<int, int> tile = tiles[index];
                tileStates[tile.Item1][tile.Item2] = TileState.Mine;
            }

            for (int i = 0; i < tileStates.Length; i++)
            {
                for (int j = 0; j < tileStates[i].Length; j++)
                {
                    if (tileStates[i][j] == TileState.Mine) continue;

                    int mineCount = GetTileCount(TileState.Mine, i, j);
                    if (mineCount == 0)
                    {
                        tileStates[i][j] = TileState.OpenNone;
                        continue;
                    }

                    tileStates[i][j] = (TileState) mineCount;
                }
            }
        }

        private void RevealFlags()
        {
            for (int i = 0; i < xTiles; i++)
            {
                for (int j = 0; j < yTiles; j++)
                {
                    if (tileStates[i][j] != TileState.Mine) continue;

                    revealedGrid[i][j] = TileState.Flagged;
                }
            }
        }

        private void RevealTiles(int x, int y)
        {
            revealedGrid[x][y] = tileStates[x][y];

            if (revealedGrid[x][y] != TileState.OpenNone) return;

            for (int i = -1; i < 2; i++)
            {
                if (x + i < 0 || x + i >= xTiles) continue;

                for (int j = -1; j < 2; j++)
                {
                    if (y + j < 0 || y + j >= yTiles) continue;
                    if (i == 0 && j == 0) continue;
                    if (revealedGrid[x + i][y + j] == TileState.Flagged || revealedGrid[x + i][y + j] == TileState.Questioned) continue;

                    if (tileStates[x + i][y + j] == TileState.OpenNone && revealedGrid[x + i][y + j] == TileState.Closed)
                    {
                        revealedGrid[x + i][y + j] = tileStates[x + i][y + j];
                        RevealTiles(x + i, y + j);
                        continue;
                    }

                    revealedGrid[x + i][y + j] = tileStates[x + i][y + j];
                }
            }
        }

        private void RevealMines(int x, int y)
        {
            for (int i = 0; i < xTiles; i++)
            {
                for (int j = 0; j < yTiles; j++)
                {
                    if (tileStates[i][j] != TileState.Mine)
                    {
                        if (revealedGrid[i][j] == TileState.Flagged)
                        {
                            revealedGrid[i][j] = TileState.NoMine;
                        }

                        continue;
                    }

                    revealedGrid[i][j] = TileState.Mine;
                }
            }

            revealedGrid[x][y] = TileState.ExMine;
        }

        private int GetTileCount(TileState state, int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                if (x + i < 0 || x + i >= xTiles) continue;

                for (int j = -1; j < 2; j++)
                {
                    if (y + j < 0 || y + j >= yTiles) continue;

                    if (tileStates[x + i][y + j] == state) count++;
                }
            }

            return count;
        }
    }
}
