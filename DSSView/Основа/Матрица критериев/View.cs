using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    /// <summary>
    /// Визуализированные и редактируемая матрица
    /// </summary>
    class ProblemMatrixView : NotifyObj, ITab
    {
        //Источник
        public ProblemPayMatrix Problem { get; private set; }


        //Представления отдельных объектов
        public MatrixView MatrixView { get; set; }
        public CriteriasReportView ReportCriterias { get; set; }
        public ProblemInformationView Information { get; set; }


        //Выбранная ячейка, строка, столбец
        public CellAdvanced SelectedCell
        {
            get => selectedCell;
            set
            {
                if(value != null && value.Coords.X > 0 && value.Coords.Y > 0)
                {
                    selectedCell = value;
                    //SelectedCellView = new CellView(Problem.Matrix,value);
                    OnPropertyChanged();
                    //OnPropertyChanged(nameof(SelectedCellView));
                }
            }
        }
        private CellAdvanced selectedCell;
        //public CellView SelectedCellView { get; set; }

        
        //Команды
        public RelayCommand AddColCommand { get; set; }
        public RelayCommand RemoveColCommand { get; set; }
        public RelayCommand AddRowCommand { get; set; }
        public RelayCommand RemoveRowCommand { get; set; }


        private void AddCol(object obj)
        {
            Problem.Matrix.AddCol(Problem.Matrix.ColsLen);
            OnPropertyChanged(nameof(Problem));
        }
        private void AddRow(object obj)
        {
            Problem.Matrix.AddRow(Problem.Matrix.RowsLen);
            OnPropertyChanged(nameof(Problem));
        }
        private void RemoveCol(object obj)
        {
            Problem.Matrix.RemoveCol(SelectedCell.ColPosition);
            OnPropertyChanged(nameof(Problem));
        }
        private void RemoveRow(object obj)
        {
            Problem.Matrix.RemoveRow(SelectedCell.RowPosition);
            OnPropertyChanged(nameof(Problem));
        }

        public ProblemMatrixView(ProblemPayMatrix data)
        {
            AddColCommand = new RelayCommand(AddCol, obj => true);
            AddRowCommand = new RelayCommand(AddRow, obj => true);
            RemoveColCommand = new RelayCommand(RemoveCol, obj => SelectedCell != null && Problem.Matrix.ColsLen > 1);
            RemoveRowCommand = new RelayCommand(RemoveRow, obj => SelectedCell != null && Problem.Matrix.RowsLen > 1);

            SetMatrix(data);
        }
        private void SetMatrix(ProblemPayMatrix data)
        {
            Problem = data;
            Children = new ObservableCollection<ITab>();
            ReportCriterias = new CriteriasReportView(data.InfoCriterias);
            Information = new ProblemInformationView(data.Info, data.Matrix);

            Children.Add(Information);
            Children.Add(ReportCriterias);   


            //if (!(data is ProblemSafeMatrix))
            //{
            //    //Children.Add(new ProblemMatrixView(data.SafeMatrix));
            //}


            OnPropertyChanged(nameof(Problem));
            OnPropertyChanged(nameof(Children));
            OnPropertyChanged(nameof(ReportCriterias));
            OnPropertyChanged(nameof(Information));
        }


        //Интерфейс вкладки
        public string Name => "мдыыыы";
        public ColorInfo Color => new ColorInfo();

        public ObservableCollection<ITab> Children { get; private set; }
    }


    class ProblemInformationView : NotifyObj, ITab
    {
        //Источник
        public Matrix Matrix { get; set; }
        public PayMatrixInfo Info { get; set; }

        //Вкладка
        public string Name => "Исходы, альтернативы";
        public ColorInfo Color => new ColorInfo();

        public Alternative[] Alternatives => Info.Alternatives.ToArray();
        public Case[] Cases => Info.Cases.ToArray();

        //Изменяемые данные
        public string Conditions => Info.Conditions;
        public bool AreChancesOk => Info.AreChancesOk;
        public double CaseChancesSum => Info.KnownChanceSum;
        public double DefaultChanceUnknown => Info.DefaultUnknownChance;

        //Выбранные альтернативы и исходы
        public Alternative SelectedAlternative { get; set; }
        public Case SelectedCase { get; set; }


        public RelayCommand AddAlternative { get; set; }
        public RelayCommand RemoveAlternative { get; set; }

        public RelayCommand RecoundChancesC { get; set; }

        public RelayCommand AddCase { get; set; }
        public RelayCommand RemoveCase { get; set; }

        private void RecountChancesExe(object obj)
        {
            Info.Cases.ForEach(c => { c.Chance = 0; c.IsChanceKnown = false; });
            OnPropertyChanged(nameof(Cases));
        }

        private void AddAlternativeExe(object obj)
        {
            Info.AddAlternative(new Alternative($"A{Info.Alternatives.Count}"));
            OnPropertyChanged(nameof(Alternatives));
        }
        private void AddCaseExe(object obj)
        {
            Info.AddCase(new Case($"C{Info.Cases.Count}",false));
            OnPropertyChanged(nameof(Cases));
        }
        private void RemoveAlternativeExe(object obj)
        {
            Info.RemoveAlternative(SelectedAlternative);
            SelectedAlternative = null;
            OnPropertyChanged(nameof(Alternatives));
        }
        private void RemoveCaseExe(object obj)
        {
            Info.RemoveCase(SelectedCase);
            SelectedCase = null;
            OnPropertyChanged(nameof(Cases));
        }

        public ProblemInformationView(PayMatrixInfo source, Matrix obj1)
        {
            Matrix = obj1;
            Info = source;
            source.ChancesChanged += Source_ChancesChanged;
            AddAlternative = new RelayCommand(AddAlternativeExe, obj => true);
            AddCase = new RelayCommand(AddCaseExe, obj => true);
            RemoveAlternative = new RelayCommand(RemoveAlternativeExe, obj => SelectedAlternative != null && Info.Alternatives.Count > 1);
            RemoveCase = new RelayCommand(RemoveCaseExe, obj => SelectedCase != null && Info.Cases.Count > 1);
            RecoundChancesC = new RelayCommand(RecountChancesExe, obj => true);
            
        }

        private void Source_ChancesChanged()
        {
            OnPropertyChanged(nameof(AreChancesOk));
            OnPropertyChanged(nameof(CaseChancesSum));
            OnPropertyChanged(nameof(DefaultChanceUnknown));
            OnPropertyChanged(nameof(Conditions));
        }
    }


    //Матрица с объектами для строк и столбцов
    class MatrixView
    {
        //Содержание матрицы
        public Matrix SourceMatrix { get; set; }

        public CellView[][] Cells
        {
            get
            {
                CellView[][] cells = new CellView[SourceMatrix.ColsLen][];
                for (int r = 0; r < SourceMatrix.ColsLen; r++)
                {
                    Cells[r] = new CellView[SourceMatrix.RowsLen];
                }

                for (int r = 0; r < SourceMatrix.RowsLen; r++)
                {
                    for (int c = 0; c < SourceMatrix.ColsLen; c++)
                    {
                        Cells[c][r] = new CellView(SourceMatrix.Cells[c][r] as CellAdvanced);
                    }
                }
                return cells;
            }
        }

        public MatrixView(Matrix matrix)
        {
            SourceMatrix = matrix;

            //SourceMatrix.StructureChanged += UpdateRowsColumns; 
        }
    }

    class CriteriasReportView : NotifyObj, ITab
    {
        //Источник
        public CriteriasReport Report { get; set; }


        //Как вкладка
        public string Name => "Отчет по критериям";
        public ColorInfo Color => new ColorInfo();
        
        //Данные
        public List<CriteriaView> Criterias { get; set; }

        public CriteriasReportView(CriteriasReport infoCriterias)
        {
            Report = infoCriterias;
            Criterias = Report.Criterias.Select(c => new CriteriaView(c)).ToList();
        }
    }
    class CriteriaView : NotifyObj, ITab
    {
        //Источник
        public Criteria Criteria { get; set; }

        //Как вкладка
        public string Name => Criteria.Name;
        public ColorInfo Color => new ColorInfo();


        //Обновляемые данные
        public double Result => Criteria.Result;
        public List<Alternative> Choices
        {
            get
            {
                List<Alternative> alts = new List<Alternative>();
                Criteria.Choices.ForEach(choiceInt => alts.Add(Criteria.Report.Matrix.Rows[choiceInt]));
                return alts;
            }
        }




        public CriteriaView(Criteria criteria)
        {
            Criteria = criteria;
            Criteria.ResultChanged += Criteria_ResultChanged;
        }
        private void Criteria_ResultChanged()
        {
            OnPropertyChanged(nameof(Result));
            OnPropertyChanged(nameof(Choices));
        }
    }

    /// <summary>
    /// Редактируемая ячейка матрицы
    /// </summary>
    class CellView : NotifyObj
    {
        public CellAdvanced Cell { get; set; }

        public bool IsEditable { get; set; }


        public CellView(CellAdvanced cell)
        {
            Cell = cell;
        }
    }
}
