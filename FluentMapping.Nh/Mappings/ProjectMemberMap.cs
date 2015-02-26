using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;

    public class ProjectMemberMap
        : EntityMap<ProjectMember>
    {
        public ProjectMemberMap()
            : base("PRJ_MEMBER")
        {
            Table("PRJ_PROJECT_MEMBER");

            References(x => x.Project, "PJ_ID").Not.Nullable().UniqueKey("KEY_PROJECT_MEMBER");
            References(x => x.User, "AUT_ID").Not.Nullable().UniqueKey("KEY_PROJECT_MEMBER");

            References(x => x.ProjectRole, "PJ_ROLE_ID").Not.Nullable();

            MapVersion();
        }
    }
}
