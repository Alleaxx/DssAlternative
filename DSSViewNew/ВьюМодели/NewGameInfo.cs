using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class NewGameInfo
    {
        public string Name { get; set; } = "Матрица";
        public int Rows { get; set; } = 3;
        public int Cols { get; set; } = 3;

        public NewGameInfo()
        {

        }
        public NewGameInfo(int rows, int cols, string name)
        {
            Rows = rows;
            Cols = cols;
            Name = name;

        }
    }
}
