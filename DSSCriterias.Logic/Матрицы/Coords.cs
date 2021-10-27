using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSCriterias.Logic
{
    //Ячейки
    public struct Coords
    {
        public override string ToString()
        {
            return $"[{X},{Y}]";
        }

        public int X { get; private set; }
        public int Y { get; private set; }

        public Coords(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Coords Of(int x, int y)
        {
            return new Coords(x, y);
        }
    }
}
