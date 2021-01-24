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
        public ProblemPayMatrix Problem { get; private set; }
        public CriteriasReportView ReportCriterias { get; set; }
        public ProblemInformationView Information { get; set; }


        //Выбранная ячейка, строка, столбец
        public Cell SelectedCell
        {
            get => selectedCell;
            set
            {
                if(value != null && value.Coords.X > 0 && value.Coords.Y > 0)
                {
                    selectedCell = value;
                    SelectedCellView = new CellView(Problem.Matrix,value);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SelectedCellView));
                }
            }
        }
        private Cell selectedCell;
        public CellView SelectedCellView { get; set; }

        
        //Команды
        public RelayCommand AddColCommand { get; set; }
        public RelayCommand RemoveColCommand { get; set; }
        public RelayCommand AddRowCommand { get; set; }
        public RelayCommand RemoveRowCommand { get; set; }


        private void AddCol(object obj)
        {
            Problem.Matrix.Info.AddCase(new Case($"C{Problem.Matrix.Colums.Length}",false));
            Problem.Matrix.SourceMatrix.AddCol(Problem.Matrix.Colums.Length);
            OnPropertyChanged(nameof(Problem));
        }
        private void AddRow(object obj)
        {
            Problem.Matrix.Info.AddAlternative(new Alternative($"A{Problem.Matrix.Rows.Length}"));
            Problem.Matrix.SourceMatrix.AddRow(Problem.Matrix.Rows.Length);
            OnPropertyChanged(nameof(Problem));
        }
        private void RemoveCol(object obj)
        {
            Problem.Matrix.SourceMatrix.RemoveCol(SelectedCellView.Col.Position - 1);
            Problem.Matrix.Info.RemoveCase(SelectedCellView.Case);
            OnPropertyChanged(nameof(Problem));
        }
        private void RemoveRow(object obj)
        {
            Problem.Matrix.SourceMatrix.RemoveRow(SelectedCellView.Row.Position - 1);
            Problem.Matrix.Info.RemoveAlternative(SelectedCellView.Alternative);
            OnPropertyChanged(nameof(Problem));
        }

        public ProblemMatrixView(ProblemPayMatrix data)
        {
            AddColCommand = new RelayCommand(AddCol, obj => true);
            AddRowCommand = new RelayCommand(AddRow, obj => true);
            RemoveColCommand = new RelayCommand(RemoveCol, obj => SelectedCell != null && SelectedCellView.Col != Problem.Matrix.Colums[0] && Problem.Matrix.SourceMatrix.ColsLen > 1);
            RemoveRowCommand = new RelayCommand(RemoveRow, obj => SelectedCell != null && SelectedCellView.Row != Problem.Matrix.Rows[0] && Problem.Matrix.SourceMatrix.RowsLen > 1);

            SetMatrix(data);
        }
        private void SetMatrix(ProblemPayMatrix data)
        {
            Problem = data;
            Children = new ObservableCollection<ITab>();
            ReportCriterias = new CriteriasReportView(data.InfoCriterias);
            Information = new ProblemInformationView(data.Matrix.Info, data.Matrix);   
            if (!(data is ProblemSafeMatrix))
            {
                Children.Add(Information);
                Children.Add(ReportCriterias);
                Children.Add(new ProblemMatrixView(data.SafeMatrix));
            }


            OnPropertyChanged(nameof(Problem));
            OnPropertyChanged(nameof(Children));
            OnPropertyChanged(nameof(ReportCriterias));
            OnPropertyChanged(nameof(Information));
        }


        //Интерфейс вкладки
        public string Name => Problem.Matrix.Name;
        public ColorInfo Color => new ColorInfo();

        public ObservableCollection<ITab> Children { get; private set; }
    }


    class ProblemInformationView : NotifyObj, ITab
    {
        //Источник
        public MatrixObject Matrix { get; set; }
        public MatrixInformation Info { get; set; }

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
            Matrix.SourceMatrix.AddRow(Matrix.SourceMatrix.RowsLen);
            OnPropertyChanged(nameof(Alternatives));
        }
        private void AddCaseExe(object obj)
        {
            Info.AddCase(new Case($"C{Info.Cases.Count}",false));
            Matrix.SourceMatrix.AddCol(Matrix.SourceMatrix.RowsLen);
            OnPropertyChanged(nameof(Cases));
        }
        private void RemoveAlternativeExe(object obj)
        {
            Matrix.SourceMatrix.RemoveRow(Info.Alternatives.IndexOf(SelectedAlternative));
            Info.RemoveAlternative(SelectedAlternative);
            SelectedAlternative = null;
            OnPropertyChanged(nameof(Alternatives));
        }
        private void RemoveCaseExe(object obj)
        {
            Matrix.SourceMatrix.RemoveCol(Info.Cases.IndexOf(SelectedCase));
            Info.RemoveCase(SelectedCase);
            SelectedCase = null;
            OnPropertyChanged(nameof(Cases));
        }

        public ProblemInformationView(MatrixInformation source, MatrixObject obj1)
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


    class MatrixObjectView
    {
        public MatrixObject Matrix { get; set; }

        public MatrixObjectView(Matrix matrix)
        {

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
                Criteria.Choices.ForEach(choiceInt => alts.Add(Criteria.Report.Matrix.Info.Alternatives[choiceInt]));
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
        public MatrixObject Matrix { get; set; }

        public Cell Cell { get; set; }

        public Alternative Alternative => Matrix.Info.Alternatives[Cell.Coords.X - 1];
        public Case Case => Matrix.Info.Cases[Cell.Coords.Y - 1];

        public MatrixRow Row => Matrix.Rows[Cell.Coords.X];
        public MatrixCol Col => Matrix.Colums[Cell.Coords.Y];


        public bool IsEditable { get; set; }

        public string Value
        {
            get => Cell.Value;
            set
            {
                Cell.Value = value;
                OnPropertyChanged();
            }
        }


        public CellView(MatrixObject data,Cell cell)
        {
            Cell = cell;
            Matrix = data;
        }
    }
}
