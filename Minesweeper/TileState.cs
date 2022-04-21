using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public enum TileState
    {
        Closed = 0,
        OpenOne = 1,
        OpenTwo = 2,
        OpenThree = 3,
        OpenFour = 4,
        OpenFive = 5,
        OpenSix = 6,
        OpenSeven = 7,
        OpenEight = 8,
        OpenNone = 9,
        Flagged = 10,
        Questioned = 11,
        Mine = 12,
        ExMine = 13,
        NoMine = 14
    }
}
