using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public static class IGameLogicFactory
    {
        public static IGameLogic Create() => new GameLogic();
    }
}
