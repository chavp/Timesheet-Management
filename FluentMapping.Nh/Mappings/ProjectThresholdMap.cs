using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class ProjectThresholdMap
        : EntityMap<ProjectThreshold>
    {
        public ProjectThresholdMap()
            : base("PJ_THRESHOLD")
        {
            Table("PRJ_PROJECT_THRESHOLD");

            Map(x => x.Name, "PJ_THRESHOLD_NAME").Length(50).Not.Nullable();
            Map(x => x.LimitRatio, "PJ_THRESHOLD_LIMIT").CustomSqlType("NUMERIC(4, 3)");

            MapVersion();
        }
    }
}
