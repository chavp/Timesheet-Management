using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class ProjectRoleRateMap
        : EntityMap<ProjectRoleRate>
    {
        public ProjectRoleRateMap()
            : base("PJ_ROLE_RATE")
        {
            Table("MST_PROJECT_ROLE_RATE");

            Map(x => x.EffectiveStart, "RATE_START_DATE").CustomType("Date");
            Map(x => x.EffectiveEnd, "RATE_END_DATE").CustomType("Date");

            Map(x => x.Cost, "RATE_PER_DAY").CustomSqlType("NUMERIC(9, 2)");

            MapVersion();
        }
    }
}
