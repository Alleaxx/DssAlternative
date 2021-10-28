using System;
using System.Collections.Generic;
using System.Text;


namespace DSSCriterias.Logic.Extensions
{
    public static class CriteriaExtensions
    {
        public static double[] NewRows(this Criteria criteria)
        {
            return new double[(int)criteria.Game.Arr.Rows()];
        }
    }
}