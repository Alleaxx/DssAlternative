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
        public override string ToString() => $"Рейтинг критерия {Rating}";

        public IEnumerable<Note> Notes { get; private set; }
        public double Rating => Notes.Sum(c => c.Profit);

        public RankCriteria(Criteria criteria, IStatGame game)
        {
            Notes = UpdateRank(criteria, game);
        }
        private IEnumerable<Note> UpdateRank(Criteria criteria, IStatGame game)
        {
            List<Note> notes = new List<Note>();

            bool chancesRequired = criteria.ChancesRequired;

            bool isOkUnknown = !chancesRequired && !game.InRiscConditions;
            bool isAnyway = !chancesRequired && game.InRiscConditions;

            bool isNotOkRisc = chancesRequired && !game.InRiscConditions;
            bool isOkRisc = chancesRequired && game.InRiscConditions;

            if (isOkUnknown)
            {
                Add("Применяется в условиях неопределенности", 3);
            }
            if (isAnyway)
            {
                Add("Может применяться и в условиях риска", 1);
            }
            if (isNotOkRisc)
            {
                Add("Не применяется в условиях неопределенности", -3);
            }
            if (isOkRisc)
            {
                Add("Применяется в условиях риска", 5);
            }

            void Add(string name, double rank)
            {
                notes.Add(new Note(name, rank));
            }

            return notes;
        }
    }
    public class Note
    {
        public override string ToString() => $"{Profit} - {Name}";

        public string Name { get; private set; }
        public double Profit { get; private set; }
        public bool IsGood => Profit > 0;

        public Note(string name, double profit)
        {
            Name = name;
            Profit = profit;
        }
    }

}
