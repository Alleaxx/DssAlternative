using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{

    //Суммарный рейтинг альтернативы по всем критериям
    public class RankAlternative
    {
        public override string ToString() => $"Рейтинг \"{Alternative}\" - {Rating}";

        public Alternative Alternative { get; private set; }
        public IEnumerable<ICriteria> Criterias { get; private set; }

        public double Rating => Criterias.Sum(c => c.Rank.Rating);
        public double RatingTotal { get; set; }

        public RankAlternative(Alternative alternative, IEnumerable<ICriteria> criterias)
        {
            Alternative = alternative;
            Criterias = criterias.ToArray();
        }
    }
}
