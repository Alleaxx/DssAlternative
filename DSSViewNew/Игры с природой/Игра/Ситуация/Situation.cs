using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class Situation
    {
        public override string ToString()
        {
            return $"Текущее состояние: {chances} | {goal} | {usage}";
        }

        public event Action OnChanged;

        private StateGoal goal;
        private StateUsage usage;
        private StateChances chances;

        public StateChances Chances
        {
            get => chances;
            set
            {
                chances = value;
                OnChanged?.Invoke();
            }
        }
        public StateUsage Usage
        {
            get => usage;
            set
            {
                usage = value;
                OnChanged?.Invoke();
            }
        }
        public StateGoal Goal
        {
            get => goal;
            set
            {
                goal = value;
                OnChanged?.Invoke();
            }
        }

        public Situation()
        {
            Chances = StateChances.Get(DSSView.Chances.Unknown);
            Goal = StateGoal.Get(Goals.MaxProfit);
            Usage = StateUsage.Get(Usages.Couple);
        }

        public IEnumerable<Note> CompareWith(Situation situation)
        {
            return new Note[]
            {
                Chances.CompareWith(situation.Chances),
                Goal.CompareWith(situation.Goal),
                Usage.CompareWith(situation.Usage)
            };
        }
    }
}
