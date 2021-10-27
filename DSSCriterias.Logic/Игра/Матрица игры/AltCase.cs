using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSCriterias.Logic
{
    public class AltCase : MtxCell<Alternative, Case, double>
    {
        public override string ToString()
        {
            return $"{Coords} ячейка статистической матрицы";
        }

        public AltCase() { }
        public AltCase(Coords coords, Alternative alt, Case cas, double val) : base(coords, alt, cas, val)
        {

        }
    }
}
