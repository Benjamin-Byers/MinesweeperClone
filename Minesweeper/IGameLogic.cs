using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace Minesweeper
{
    public interface IGameLogic
    {
        void NewGame(GameLevel level);
        void NewGame(int xTiles, int yTiles, int mines);
        int GetRemainingMines();
        TileState[][] GetTileStates();
        void ToggleQuestionMarks();
        GameState GetGameState();
        TileState[][] SendMove(int x, int y, MoveType moveType);
    }
}