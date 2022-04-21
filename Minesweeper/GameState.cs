using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public enum GameState
    {
        Invalid = -2,
        Ready = -1,
        Lose = 0,
        Playing = 1,
        Win = 2
    }
}
