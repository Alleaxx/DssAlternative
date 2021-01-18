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
    class InfoCriterias
    {
        public List<Criteria> Criterias { get; set; }
        public Data Data { get; set; }
        public bool Safe { get; set; }

        public InfoCriterias(Data data)
        {
            Data = data;
            Criterias = new List<Criteria>();
            CountCriterias();
        }

        private void CountCriterias()
        {
            Criterias.Add(new CriteriaView(new CriteriaWald(this)));
            Criterias.Add(new CriteriaView(new CriteriaPlayer(this)));
            Criterias.Add(new CriteriaView(new CriteriaBaiesLaplas(this,false)));
            Criterias.Add(new CriteriaView(new CriteriaBaiesLaplas(this,true)));
            Criterias.Add(new CriteriaView(new CriteriaSavige(this)));
            Criterias.Add(new CriteriaView(new CriteriaGurvits(this)));
            Criterias.Add(new CriteriaView(new CriteriaLeman(this)));
            Criterias.Add(new CriteriaView(new CriteriaMulti(this)));
            Criterias.Add(new CriteriaView(new CriteriaGerr(this)));
        }
        public void Update()
        {
            Criterias.ForEach(c => c.Count());
        }
    }
    class InfoCriteriasView : InfoCriterias, ITab
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public DataView View => Data as DataView;

        public string Name => "Отчет по критериям";
        public string Tooltip => $"{Criterias.Count} подотчетов";
        public ColorInfo Color => new ColorInfo();
        public object Object => this;

        public InfoCriteriasView(DataView data) : base(data)
        {

        }


        public ObservableCollection<CriteriaView> CriteriasView => new ObservableCollection<CriteriaView>(Criterias.Cast<CriteriaView>());
        public void Add(ITab tab)
        {
            throw new NotImplementedException();
        }
        public void Remove(ITab tab)
        {
            throw new NotImplementedException();
        }
    }


    //Критерий (интерфейс компонента)
    abstract class Criteria
    {
        public virtual string Name { get; set; }
        public virtual InfoCriterias Source { get; set; }


        protected double ChanceBasic => 1 / (double)Source.Data.Cols;

        //Быстрый доступ
        protected double[,] Matrix => Source.Data.Matrix;
        protected double Rows => Source.Data.Rows;
        protected double Cols => Source.Data.Cols;



        public virtual double Result { get; set; }
        public virtual List<int> Choices { get; set; }
        public Criteria(InfoCriterias data)
        {
            Source = data;
            Choices = new List<int>();
            Count();
        }

        public abstract void Count();

        protected double GetChance(int col)
        {
            return ChanceBasic;
        }


        /// <summary>
        /// Возвращает минимальное значение из указанного столбца
        /// </summary>
        /// <param name="c">Номер колонки</param>
        /// <returns>Минимальное значение из колокни</returns>
        protected double GetMinFromCol(int c)
        {
            double min = Matrix[0,c];
            for (int r = 0; r < Rows; r++)
            {
                if (Matrix[r, c] <= min)
                    min = Matrix[r,c];
            }
            return min;
        }

        /// <summary>
        /// Возвращает минимальное значение из указанной строки
        /// </summary>
        /// <param name="r">Номер строки</param>
        /// <returns>Минимальное значение из строки</returns>
        protected double GetMinFromRow(int r)
        {
            double min = Matrix[r, 0];
            for (int c = 0; c < Cols; c++)
            {
                if (Matrix[r, c] <= min)
                    min = Matrix[r,c];
            }
            return min;
        }

        /// <summary>
        /// Возвращает максимальное значение из указанной колонки
        /// </summary>
        /// <param name="c">Номер колонки</param>
        /// <returns>Максимальное значение</returns>
        protected double GetMaxFromCol(int c)
        {
            double max = Matrix[0, c];
            for (int r = 0; r < Rows; r++)
            {
                if (Matrix[r, c] >= max)
                    max = Matrix[r, c];
            }
            return max;
        }

        /// <summary>
        /// Возвращает максимальное значение из указанной строки
        /// </summary>
        /// <param name="r">Номер строки</param>
        /// <returns>Максимальное значение</returns>
        protected double GetMaxFromRow(int r)
        {
            double max = Matrix[r, 0];
            for (int c = 0; c < Cols; c++)
            {
                if (Matrix[r, c] >= max)
                    max = Matrix[r, c];
            }
            return max;
        }


        protected List<int> GetPositions(double search, IEnumerable<double> arr)
        {
            List<int> poses = new List<int>();
            for (int i = 0; i < arr.Count(); i++)
            {
                if (search == arr.ElementAt(i))
                    poses.Add(i);
            }
            return poses;
        }
    }


    //Компоненты
    class CriteriaWald : Criteria
    {
        public CriteriaWald(InfoCriterias data) : base(data)
        {
            Name = "Критерий Вальда";
        }
        public override void Count()
        {
            List<double> mins = new List<double>();
            for (int i = 0; i < Rows; i++)
            {
                mins.Add(GetMinFromRow(i));
            }
            Result = mins.Max();
            Choices = GetPositions(Result, mins);
        }
    }
    class CriteriaPlayer : Criteria
    {
        public CriteriaPlayer(InfoCriterias data) : base(data)
        {
            Name = "Критерий азартного игрока";
        }
        public override void Count()
        {
            List<double> maxes = new List<double>();
            for (int i = 0; i < Rows; i++)
            {
                maxes.Add(GetMaxFromRow(i));
            }
            Result = maxes.Max();
            Choices = GetPositions(Result, maxes);
        }
    }
    class CriteriaBaiesLaplas : Criteria
    {
        public bool Laplas { get; set; }
        public CriteriaBaiesLaplas(InfoCriterias data, bool laplas) : base(data)
        {
            if (laplas)
                Name = "Критерий Лапласа";
            else
                Name = "Критерий Байеса";
            Laplas = laplas;
        }

        public override void Count()
        {
            double[] averages = new double[(int)Rows];
            for (int r = 0; r < Rows; r++)
            {
                double sum = 0;
                for (int c = 0; c < Cols; c++)
                {
                    if (!Laplas)
                        sum += Matrix[r, c] * GetChance(c);
                    else
                        sum += Matrix[r, c];
                }
                if (!Laplas)
                    averages[r] = sum;
                else
                    averages[r] = sum / Cols;
            }
            Result = averages.Max();
            Choices = GetPositions(Result, averages);
        }
    }
    class CriteriaSavige : Criteria
    {
        public CriteriaSavige(InfoCriterias data) : base(data)
        {
            Name = "Критерий Сэвиджа";
        }

        public override void Count()
        {
            double[] maxesInCols = new double[Source.Data.Cols];
            for (int c = 0; c < Cols; c++)
            {
                double max = double.MinValue;
                for (int r = 0; r < Rows; r++)
                {
                    if (Matrix[r, c] > max)
                        max = Matrix[r, c];
                }
                maxesInCols[c] = max;
            }


            double[] maxInRowsAfter = new double[Source.Data.Rows];
            double[,] newArr = new double[Source.Data.Rows, Source.Data.Cols];

            for (int r = 0; r < Rows; r++)
            {
                double max = double.MinValue;
                for (int c = 0; c < Cols; c++)
                {
                    newArr[r, c] = maxesInCols[c] - Matrix[r, c];
                    if (newArr[r, c] > max)
                        max = newArr[r, c];
                }
                maxInRowsAfter[r] = max;
            }
            Result = maxInRowsAfter.Min();
            Choices = GetPositions(Result, maxInRowsAfter);
        }
    }



    class CriteriaGurvits : Criteria
    {
        public double GurvitsCoeff { get; set; } = 0.4; //коэффициент пессимизма
        public CriteriaGurvits(InfoCriterias data) : base(data)
        {
            Name = "Критерий Гурвица";
        }

        public override void Count()
        {
            double[] gurv = new double[Source.Data.Rows];

            for (int r = 0; r < Rows; r++)
            {
                double max = GetMaxFromRow(r);
                double min = GetMinFromRow(r);

                gurv[r] =  min * GurvitsCoeff + max * (1 - GurvitsCoeff);
            }
            Result = gurv.Min();
            Choices = GetPositions(Result, gurv);
        }
    }
    class CriteriaLeman : Criteria
    {
        public double LemanCoeff { get; set; } = 0.6; //коэффициент доверия к информации
        public CriteriaLeman(InfoCriterias data) : base(data)
        {
            Name = "Критерий Ходжа-Лемана";
        }

        public override void Count()
        {
            double[] coeff = new double[Source.Data.Rows];

            for (int r = 0; r < Rows; r++)
            {
                double min = Matrix[r, 0];
                double sum = 0;
                for (int c = 0; c < Cols; c++)
                {
                    if (Matrix[r, c] < min)
                        min = Matrix[r, c];
                    sum += Matrix[r, c] * GetChance(c);
                }
                coeff[r] = sum * LemanCoeff + (1 - LemanCoeff) * min;
            }
            Result = coeff.Max();
            Choices = GetPositions(Result, coeff);
        }
    }
    class CriteriaMulti : Criteria
    {
        public CriteriaMulti(InfoCriterias data) : base(data)
        {
            Name = "Критерий произведений";
        }

        public override void Count()
        {
            double[] multi = new double[Source.Data.Rows];
            for (int r = 0; r < Rows; r++)
            {
                double sum = Matrix[r, 0];
                for (int c = 1; c < Cols; c++)
                {
                    sum *= Matrix[r, c];
                }
                multi[r] = sum;
            }
            Result = multi.Max();
            Choices = GetPositions(Result, multi);
        }
    }
    class CriteriaGerr : Criteria
    {
        public CriteriaGerr(InfoCriterias data) : base(data)
        {
            Name = "Критерий Гермейера";
        }

        public override void Count()
        {
            double[,] newArr = new double[Source.Data.Rows, Source.Data.Cols];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    newArr[r, c] = Matrix[r, c] > 0 ? Matrix[r, c] / GetChance(c) : Matrix[r, c] * GetChance(c);
                }
            }

            double[] minInRows = new double[Source.Data.Rows];
            for (int r = 0; r < Rows; r++)
            {
                double m = newArr[r, 0];
                for (int c = 1; c < Cols; c++)
                {
                    if (newArr[r, c] < m)
                    {
                        m = newArr[r, c];
                    }
                }
                minInRows[r] = m;
            }
            Result = minInRows.Max();
            Choices = GetPositions(Result, minInRows);
        }
    }



    //Декоратор
    class CriteriaView : Criteria, ITab
    {
        public Criteria Criteria { get; set; }

        public override string Name { get => Criteria.Name; set => Criteria.Name = value; }
        public override double Result { get => Criteria.Result; set => Criteria.Result = value; }
        public override List<int> Choices { get => Criteria.Choices; }


        public string Tooltip => "Расчет успешен";
        public ColorInfo Color => new ColorInfo();
        public object Object => this;


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        public CriteriaView(Criteria criteria) : base(criteria.Source)
        {
            Criteria = criteria;
        }
        public override void Count()
        {
            if(Criteria != null)
            {
                Criteria.Count();
                OnPropertyChanged(nameof(Result));
                OnPropertyChanged(nameof(Choices));
            }
        }

        public void Add(ITab tab)
        {
            throw new NotImplementedException();
        }
        public void Remove(ITab tab)
        {
            throw new NotImplementedException();
        }
    }






}
