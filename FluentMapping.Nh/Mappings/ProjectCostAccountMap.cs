using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class ProjectCostAccountMap
        : EntityMap<ProjectCostAccount>
    {
        public ProjectCostAccountMap()
            : base("ACC_PRJ_COST")
        {
            Table("ACC_PRJ_COST");

            Map(x => x.EffortBalanceHrs, "EFFORT_BALANCE_HRS");
            Map(x => x.CostBalance, "COST_BALANCE");
            Map(x => x.MembersBalance, "MEMBERS_BALANCE");

            References(x => x.Project, "PJ_ID").Not.Nullable().Unique();

            HasMany(x => x.Entries)
                .KeyColumn("ACC_PRJ_COST_ID")
                .Cascade
                .All()
                .Inverse()
                .LazyLoad();

            MapVersion();
        }
    }
}
