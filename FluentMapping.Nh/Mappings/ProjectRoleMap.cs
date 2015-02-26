using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class ProjectRoleMap
        : EntityMap<ProjectRole>
    {
        public ProjectRoleMap()
            : base("PJ_ROLE")
        {
            Table("MST_PROJECT_ROLE");

            Map(x => x.NameEN, "PJ_ROLE_NAME_EN").Length(100).Not.Nullable().Unique();
            Map(x => x.NameTH, "PJ_ROLE_NAME_TH").Length(100).Not.Nullable().Unique();
            Map(x => x.Order, "PJ_ROLE_ORDER");
            Map(x => x.IsOwner, "PJ_ROLE_OWNER");

            Map(x => x.IsNonRole, "PJ_NONROLE_FLAG");

            HasMany(x => x.ProjectRoleRates)
                .KeyColumn("PJ_ROLE_ID")
                .Cascade
                .All()
                .LazyLoad();

            MapVersion();
        }
    }
}
