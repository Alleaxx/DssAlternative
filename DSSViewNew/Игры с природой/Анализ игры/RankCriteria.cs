using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    //Применимость критерия
    public class RankCriteria
    {
        public override string ToString()
        {
            return $"Рейтинг критерия {Rating}";
        }

        public IEnumerable<Note> Notes { get; private set; }
        public double Rating => Notes.Sum(c => c.Profit);

        public RankCriteria(ICriteria criteria, IStatGame game)
        {
            Notes = criteria.Situation.CompareWith(game.Situation);
        }
    }
}
