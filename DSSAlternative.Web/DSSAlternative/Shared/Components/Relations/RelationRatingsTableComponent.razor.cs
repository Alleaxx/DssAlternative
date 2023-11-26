using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;
using DSSAlternative.AHP.Logs;
using DSSAlternative.AHP.MatrixMethods;
using DSSAlternative.AHP.Ratings;
using DSSAlternative.Web.AppComponents;

using Microsoft.AspNetCore.Components;

namespace DSSAlternative.Web.Shared.Components.Relations
{
    public partial class RelationRatingsTableComponent : DSSComponentRelationV2
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
            bool isRelationKnown = Relations[Relation.Main].Known;
            bool isOneRelationUnknown = Relations[Relation.Main].NodeComparesMini.Count(n => n.Unknown) == 1;

            if (UseSafeWarnings && (isRelationKnown || isOneRelationUnknown))
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
    }
}
