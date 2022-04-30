using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor
{
    public enum Axis : int
    {
        Backward = -3,
        Forward = 3,
        Down = -2,
        Up = 2,
        Left = -1,
        Right = 1,
        Center = 0,
        Main = 4,
        Other = 5,
    }
}
