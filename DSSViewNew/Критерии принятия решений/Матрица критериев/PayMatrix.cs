using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using DSSLib;

namespace DSSView
{
    public abstract class PayMatrix : MatrixContainer<DSSLib.Alternative, Case, double>, IMatrixChance<Alternative, Case,double>
    {
        public event Action CaseChanceChanged;

        public IInfoMatrix Info { get; set; }
        public ReportCriterias Report { get; set; }

        protected override void AddColToList(int pos)
        {
            base.AddColToList(pos);
            Case last = ColsList.Last();
            last.ChanceChanged += UpdateChances;
        }
        protected override void RemoveColFromList(Case col)
        {
            base.RemoveColFromList(col);
            col.ChanceChanged -= UpdateChances;
        }
        protected void UpdateChances()
        {
            CaseChanceChanged?.Invoke();
        }

        protected override void CreateFiller()
        {
            Filler = new FillerPayMatrix(this);
        }

        public PayMatrix(int rows, int cols) : base(rows, cols)
        {
            Info = new InfoPayMatrix(this);
            Report = new ReportCriterias(this);
        }
        public PayMatrix(PayMatrixXml xml) : base(0,0)
        {
            RowsList = new List<Alternative>(xml.Alternatives);
            ColsList = new List<Case>(xml.Cases);
            Arr = new double[xml.Alternatives.Length, xml.Cases.Length];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    Arr[r, c] = xml.Values[r][c];
                }
            }

            UpdateCells();

            Info = new InfoPayMatrix(this);
            Report = new ReportCriterias(this);
        }

    }

    //Риска
    public class PayMatrixRisc : PayMatrix
    {
        public PayMatrixRisc(int rows, int cols) : base(rows, cols) { }
        public PayMatrixRisc(PayMatrixXml xml) : base(xml) { }
    }
    public class PayMatrixSafe : PayMatrix
    {
        public IMatrix<Alternative, Case, double> MainMatrix { get; set; }

        public PayMatrixSafe(PayMatrixRisc matrix) : base(matrix.Rows, matrix.Cols)
        {
            MainMatrix = matrix;

            MainMatrix.ValuesChanged += c => UpdateSafeMatrixFromMain();
            MainMatrix.RowChanged += c => UpdateSafeMatrixFromMain();
            MainMatrix.ColChanged += c => UpdateSafeMatrixFromMain();
            UpdateSafeMatrixFromMain();
        }

        private void UpdateSafeMatrixFromMain()
        {
            RowsList.Clear();
            for (int r = 0; r < MainMatrix.Rows; r++)
            {
                RowsList.Add(MainMatrix.GetRow(r));
            }
            ColsList.Clear();
            for (int c = 0; c < MainMatrix.Cols; c++)
            {
                ColsList.Add(MainMatrix.GetCol(c));
            }


            Arr = new double[Rows, Cols];
            double max = double.MinValue;
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    Arr[r, c] = MainMatrix.Get(r, c);
                    if (Arr[r, c] > max)
                        max = Arr[r, c];
                }
            }

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    Arr[r, c] = Arr[r, c] * -1 + max;
                }
            }
            UpdateCells();
            Update();
        }
    }


    public class FillerPayMatrix : IRowColFiller<DSSLib.Alternative, Case, double>
    {
        private IMatrix<DSSLib.Alternative,Case,double> Matrix { get; set; }
        public FillerPayMatrix(IMatrix<DSSLib.Alternative,Case,double> matrix)
        {
            Matrix = matrix;
        }


        public CellContainer<DSSLib.Alternative, Case, double> GetNewCell(Coords coords)
            => new Cell(Matrix, coords);

        public Case GetNewColumn()
            => new Case($"C{Matrix.Cols}");

        public DSSLib.Alternative GetNewRow()
            => new DSSLib.Alternative($"A{Matrix.Rows}");

        public double GetNewValue()
            => default;
    }
    public class InfoPayMatrix : IInfoMatrix
    {
        public event Action ChancesChanged;

        private IMatrix<DSSLib.Alternative,Case,double> Matrix { get; set; }
        private Case[] Cases => Matrix.ColsArr;


        public bool InUnknownConditions
        {
            get => inUknownConditions;
            set
            {
                inUknownConditions = value;
                ChancesChanged?.Invoke();
            }
        }
        private bool inUknownConditions = true;
        public bool InRiscConditions => !InUnknownConditions;

        public double GetChance(int col)
        {
            if (InRiscConditions)
                return Matrix.GetCol(col).Chance;
            else
                return DefaultChance;
        }

        public bool AreChancesOk
        {
            get
            {
                if (InUnknownConditions)
                    return true;

                double sum = ChancesSum;
                return sum >= 0.99 && sum <= 1;
            }
        }
        public double ChancesSum => InUnknownConditions ? 1 : Cases.Select(c => c.Chance).Sum();

        public double DefaultChance => (double)1 / Cases.Length;

        
        public InfoPayMatrix(IMatrix<DSSLib.Alternative,Case,double> data)
        {
            Matrix = data;
            Matrix.RowChanged += r => InnerCase_ChanceChanged();
            Matrix.ColChanged += c => InnerCase_ChanceChanged();

        }
        private void InnerCase_ChanceChanged()
        {
            ChancesChanged?.Invoke();
        }
    }
    public class Cell : CellContainer<DSSLib.Alternative,Case,double>
    {
        public Cell(IMatrix<Alternative,Case,double> matrix, Coords coords) : base(matrix, coords) { }
    }
}
