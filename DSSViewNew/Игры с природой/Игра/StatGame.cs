using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    //Статистическая игра
    public class StatGame : IStatGame
    {
        public override string ToString() => $"Статистическая игра \"{Name}\"";

        public event Action OnInfoUpdated;

        public string Name { get; set; }
        public MtxStat Mtx { get; set; }
        public StatGameAnalysis Report { get; set; }
        public GameState State { get; set; }


        public StatGame() : this("Природа", new MtxStat())
        {

        }
        public StatGame(string name, MtxStat mtx)
        {
            Name = name;
            Mtx = mtx;

            State = new GameState(this);
            Report = new StatGameAnalysis(this);

            Mtx.OnRowChanged += r => MtxUpdated();
            Mtx.OnColChanged += c => MtxUpdated();
            Mtx.OnValuesChanged += c => MtxUpdated();
            State.OnCaseChanceChanged += c => StateUpdated();
        }
        public StatGame(StatGameXml xml) : this(xml.Name, MtxStat.CreateFromXml(xml))
        {
            State.Risc = xml.RiscConditions;
        }



        private void MtxUpdated()
        {
            OnInfoUpdated?.Invoke();
        }
        private void StateUpdated()
        {
            OnInfoUpdated?.Invoke();
        }
        private void Updated()
        {
            OnInfoUpdated?.Invoke();
        }


        //Реализация StatGame
        public double[,] Arr => Mtx.Values;
        public bool InRiscConditions => State.Risc;
        public bool InUnknownConditions => State.Unknown;
        public double GetChance(int col) => State.GetChance(col);


        public double Get(int r, int c) => Mtx.Get(r, c);
        public Alternative GetRow(int r) => Mtx.GetRow(r);
    }
    public class GameState
    {
        public override string ToString() => $"Состояние игры: {Status}";
        public event Action<Case> OnCaseChanceChanged;

        private StatGame Game { get; set; }
        private IEnumerable<Case> Cases => Game.Mtx.Cols;


        public double SumCases => Cases.Select(c => GetChance(c)).Sum();
        public bool IsOk => SumCases == 1;
        public bool Risc
        {
            get => risc;
            set
            {
                risc = value;
                OnCaseChanceChanged?.Invoke(null);
            }
        }
        private bool risc;
        public bool Unknown => !Risc;
        public string Status => IsOk ? Risc ? "Условия риска" : "Условия неизвестности" : "Некорректные условия";
        public double GetChance(Case c)
        {
            if (Unknown)
                return 1 / (double)Cases.Count();
            else
                return c.Chance;
        }
        public double GetChance(int pos)
        {
            Case c = Cases.ElementAt(pos);
            return GetChance(c);
        }


        public GameState(StatGame game)
        {
            Game = game;
            game.Mtx.OnColChanged += Mtx_OnColChanged;
            foreach (var c in Cases)
            {
                c.OnChanceChanged += Case_OnChanceChanged;
            }
        }

        private void Case_OnChanceChanged()
        {
            OnCaseChanceChanged?.Invoke(null);
        }


        private void Mtx_OnColChanged(Case obj)
        {
            obj.OnChanceChanged += Case_OnChanceChanged;
        }
    }

}
