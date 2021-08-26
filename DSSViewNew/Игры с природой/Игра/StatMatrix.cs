using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    //Матрица статистической игры
    public class MtxStat : Matrix<Alternative, Case, double>
    {
        public override string ToString() => $"{RowsCount}x{ColsCount} матрица статистической игры";

        public MtxStat() : base(new MtxStatFactory())
        {

        }
        public static MtxStat CreateDefault() => default;
        public static MtxStat CreateFromSize(int rows, int cols)
        {
            MtxStat newMtx = new MtxStat();
            for (int i = 0; i < rows - 1; i++)
            {
                newMtx.AddRow();
            }
            for (int i = 0; i < cols - 1; i++)
            {
                newMtx.AddCol();
            }
            return newMtx;
        }
        public static MtxStat CreateFromArr(double[,] arr) => default;
        public static MtxStat CreateFromXml(StatGameXml xml)
        {
            MtxStat mtx = new MtxStat();
            mtx.Clear();
            for (int r = 0; r < xml.Values.Count; r++)
            {
                Alternative alt = xml.Alternatives[r];
                var row = xml.Values[r];
                for (int c = 0; c < row.Length; c++)
                {
                    Case cas = xml.Cases[c];
                    double val = row[c];
                    AltCase cell = new AltCase() { Coords = new Coords(r, c), From = alt, To = cas, Value = val };
                    mtx.Add(cell);
                }
            }
            return mtx;
        }
        public static MtxStat CreateFromSafe(MtxStat mtx) => default;
        public static MtxStat CreateFromRisc(MtxStat mtx) => default;

    }
    public class AltCase : MtxCell<Alternative, Case, double>
    {
        public override string ToString() => $"{Coords} ячейка статистической матрицы";
    }
    public class MtxStatFactory : MtxCellFactory<Alternative, Case, double>
    {
        public override string ToString() => $"Фабрика статистической игры";
        public override MtxCell<Alternative, Case, double> NewCell()
            => new AltCase() { Coords = new Coords(0, 0), From = NewRow, To = NewCol, Value = NewValue };

        public override MtxCell<Alternative, Case, double> NewCell(Coords coords, Alternative r, Case c, double v)
            => new AltCase() { Coords = coords, From = r, To = c, Value = v };


        public override Case NewCol => new Case("С");
        public override Alternative NewRow => new Alternative("А");
        public override double NewValue => 0;
    }


    //СМ. также критерий Сэвиджа
    //Преобразование в матрицу рисков (или обратно)
    public class SafeMatrix
    {
        //private void UpdateSafeMatrixFromMain()
        //{
        //    RowsList.Clear();
        //    for (int r = 0; r < MainMatrix.Rows; r++)
        //    {
        //        RowsList.Add(MainMatrix.GetRow(r));
        //    }
        //    ColsList.Clear();
        //    for (int c = 0; c < MainMatrix.Cols; c++)
        //    {
        //        ColsList.Add(MainMatrix.GetCol(c));
        //    }


        //    Arr = new double[Rows, Cols];
        //    double max = double.MinValue;
        //    for (int r = 0; r < Rows; r++)
        //    {
        //        for (int c = 0; c < Cols; c++)
        //        {
        //            Arr[r, c] = MainMatrix.Get(r, c);
        //            if (Arr[r, c] > max)
        //                max = Arr[r, c];
        //        }
        //    }

        //    for (int r = 0; r < Rows; r++)
        //    {
        //        for (int c = 0; c < Cols; c++)
        //        {
        //            Arr[r, c] = Arr[r, c] * -1 + max;
        //        }
        //    }
        //    UpdateCells();
        //    Update();
        //}
    }


}
