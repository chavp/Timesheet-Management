using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;

    public class ProjectStatusMap
        : EntityMap<ProjectStatus>
    {
        public ProjectStatusMap()
            : base("PRJ_STATUS")
        {
            Table("PRJ_PROJECT_STATUS");

            Map(x => x.Name, "PRJ_STATUS_NAME").Length(30).Not.Nullable().Unique();

            MapVersion();
        }
    }
}
