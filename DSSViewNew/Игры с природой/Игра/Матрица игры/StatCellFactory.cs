using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class MtxStatFactory : MtxCellFactory<Alternative, Case, double>
    {
        public override string ToString()
        {
            return $"Фабрика статистической игры";
        }

        public override MtxCell<Alternative, Case, double> NewCell()
        {
            return new AltCase(Coords.Of(0, 0), NewRow(0), NewCol(0), NewValue);
        }

        public override MtxCell<Alternative, Case, double> NewCell(Coords coords, Alternative r, Case c, double v)
        {
            return new AltCase(coords, r, c, v);
        }

        public override Case NewCol(int index)
        {
            return new Case($"С{index}");
        }
        public override Alternative NewRow(int index)
        {
            return new Alternative($"А{index}");
        }
        public override double NewValue => 0;
    }
}
