using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;
using DSSAlternative.AppComponents;

using Microsoft.AspNetCore.Components;

namespace DSSAlternative.Shared.Components
{
    public partial class RelationQuestion : DSSComponentParamRelation
    {
        [Parameter]
        public EventCallback RelationUpdated { get; set; }
        [Parameter]
        public bool UseSafeWarnings { get; set; } = true;


        private IRating RatingNone { get; set; }
        private IRating RatingEqual { get; set; }
        private IEnumerable<(IRating forRating, IRating toRating)> Ratings { get; set; }

        private Dictionary<IRating, IMatrix> RatingMatrix { get; set; } = new Dictionary<IRating, IMatrix>();
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            CreateRatings();
            CreateMatrixes();
        }
        private void CreateRatings()
        {
            RatingNone = new Rating(0);
            RatingEqual = new Rating(1);
            Ratings = RatingSystem.RatingsFor(Relation);
        }
        private void CreateMatrixes()
        {
            if(UseSafeWarnings && Relations[Relation.Main].Known)
            {
                RatingMatrix.Add(RatingNone, GetMatrixForRating(0));
                RatingMatrix.Add(RatingEqual, GetMatrixForRating(1));
                foreach (var rating in Ratings)
                {
                    double value = rating.forRating.Value;

                    RatingMatrix.Add(rating.forRating, GetMatrixForRating(value));
                    RatingMatrix.Add(rating.toRating, GetMatrixForRating(1 / value));
                }

                IMatrix GetMatrixForRating(double value)
                {
                    IMatrix source = Relations[Relation.Main].Mtx;
                    source.Change(Relation.From, Relation.To, value);
                    return source;
                }
            }
        }
        public string GetTextRelation(INodeRelation relation)
        {
            if (relation.Unknown)
                return "??????";
            double val = relation.Value > 1 ? relation.Value : 1 / relation.Value;
            if (val == 1)
                return $"РАВНОЗНАЧЕН";
            if (val >= 9)
                return "АБСОЛЮТНО ПРЕВОСХОДИТ";
            else if (val >= 8)
                return "СИЛЬНО ПРЕОБЛАДАЕТ над";
            else if (val >= 7)
                return "СИЛЬНО ПРЕОБЛАДАЕТ над";
            else if (val >= 6)
                return "ПРЕОБЛАДАЕТ над";
            else if (val >= 5)
                return "ПРЕОБЛАДАЕТ над";
            else if (val >= 4)
                return "НЕМНОГО ПРЕОБЛАДАЕТ над";
            else if (val >= 3)
                return "НЕМНОГО ПРЕОБЛАДАЕТ над";
            else if (val >= 2)
                return "СЛЕГКА ПРЕОБЛАДАЕТ над";
            else
                return "СЛЕГКА ПРЕОБЛАДАЕТ над";
        }
    }
}
