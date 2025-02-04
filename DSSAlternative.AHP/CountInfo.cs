using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{

    /// <summary>
    /// Информация о прогрессе / процессе: всего, отфильтровано, соотношение
    /// </summary>
    public class CountInfo
    {
        public readonly int Filtered;
        public readonly int Total;

        public double Coefficient => (double)Filtered / Total;
        public double Percentage => Coefficient * 100.0;

        public CountInfo(int filtered, int total)
        {
            Filtered = filtered;
            Total = total;
        }
    }
}
