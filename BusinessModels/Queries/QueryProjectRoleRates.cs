using Cwn.PM.BusinessModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Queries
{
    public static class QueryProjectRoleRates
    {
        public static IEnumerable<decimal> GetEffectiveRatePerHours(this IList<ProjectRoleRate> projectRoleRates, DateTime? actualStartDate)
        {
            return from c in projectRoleRates
                   where c.EffectiveStart.GetValueOrDefault(DateTime.MinValue) <= actualStartDate
                   && actualStartDate <= c.EffectiveEnd.GetValueOrDefault(DateTime.MaxValue)
                   select c.Cost / 8;
        }


    }
}
