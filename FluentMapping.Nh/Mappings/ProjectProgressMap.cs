using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;

    public class ProjectProgressMap
        : EntityMap<ProjectProgress>
    {
        public ProjectProgressMap()
            : base("PRJ_PROGRESS")
        {
            Table("PRJ_PROGRESS");

            //Map(x => x.PercentageProgress, "PERCENTAGE_PROGRESS").CustomSqlType("NUMERIC(3, 0)");
            
            References(x => x.Project, "PJ_ID").Not.Nullable().Unique();
  
            HasMany(x => x.Entries)
                .KeyColumn("PRJ_PROGRESS_ID")
                .Cascade
                .All()
                .Inverse()
                .LazyLoad();

            MapVersion();
        }
    }
}
