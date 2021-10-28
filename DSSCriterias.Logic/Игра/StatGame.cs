using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSCriterias.Logic
{
    //Статистическая игра
    public interface IStatGame
    {
        event Action OnInfoUpdated;
        event Action OnStructureChanged;
        double[,] Arr { get; }
        Situation Situation { get; }
        double GetChance(int col);
        void SetChance(int col, double value);
        Alternative GetRow(int r);
    }

    public class StatGame : IStatGame
    {
        public override string ToString()
        {
            return $"Статистическая игра \"{Name}\"";
        }

        public event Action OnInfoUpdated;
        public event Action OnStructureChanged;

        public string Name { get; set; }

        public MtxStat Mtx { get; private set; }
        public double[,] Arr => Mtx.Values;
        public Alternative GetRow(int r)
        {
            return Mtx.GetRow(r);
        }

        public GameAnalysis Report { get; private set; }

        public Situation Situation { get; private set; }
        public double GetChance(int pos)
        {
            return Situation.Chances.GetChance(Mtx.Cols, pos);
        }
        public void SetChance(int col, double value)
        {
            Mtx.Cols[col].Chance = value;
        }

        //Стандартная игра
        public StatGame() : this("Природа", new MtxStat())
        {

        }
        //Из файла
        public StatGame(StatGameXml xml) : this(xml.Name, MtxStat.CreateFromXml(xml))
        {

        }
        //Настраиваемая игра
        public StatGame(string name, MtxStat mtx) : this(name, mtx, new Situation())
        {

        }
        //Настраиваемая игра
        public StatGame(string name, MtxStat mtx, Situation situation)
        {
            Name = name;
            Mtx = mtx;
            Situation = situation;
            Report = new GameAnalysis(this);

            AddListeners();
            SetCaseListeners();

            void SetCaseListeners()
            {
                foreach (Case cas in Mtx.Cols)
                {
                    cas.OnChanceChanged += CaseChanceChanged;
                }
            }
            void AddListeners()
            {
                Situation.OnChanged += MtxUpdated;
                Mtx.OnRowRemoved += r => MtxUpdated();
                Mtx.OnRowAdded += r => MtxUpdated();

                Mtx.OnColAdded += c => MtxUpdated();
                Mtx.OnColRemoved += c => MtxUpdated();
                Mtx.OnValuesChanged += c => MtxUpdated();

                Mtx.OnColAdded += CaseValueAddListener;
                Mtx.OnColRemoved += CaseValueRemoveListener;
            }
        }

        private void MtxUpdated()
        {
            OnInfoUpdated?.Invoke();
        }

        private void CaseChanceChanged()
        {
            MtxUpdated();
        }
        private void CaseValueAddListener(Case c)
        {
            c.OnChanceChanged += CaseChanceChanged;
        }
        private void CaseValueRemoveListener(Case c)
        {
            c.OnChanceChanged -= CaseChanceChanged;
        }

    }
}
