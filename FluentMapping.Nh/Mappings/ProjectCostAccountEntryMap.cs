using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class ProjectCostAccountEntryMap
        : EntityMap<ProjectCostAccountEntry>
    {
        public ProjectCostAccountEntryMap()
            : base("ACC_PRJ_COST_ENTRY")
        {
            Table("ACC_PRJ_COST_ENTRY");

            References(x => x.ProjectCostAccount, "ACC_PRJ_COST_ID").Not.Nullable().UniqueKey("KEY_ACC_PRJ_COST_ENTRY");
            Map(x => x.StartDate, "TME_START_DATE").CustomType("Date").UniqueKey("KEY_ACC_PRJ_COST_ENTRY");

            Map(x => x.EffortHrs, "EFFORT_HRS");
            Map(x => x.Members, "MEMBERS");
            Map(x => x.Cost, "COST");

            MapVersion();
        }
    }
}
