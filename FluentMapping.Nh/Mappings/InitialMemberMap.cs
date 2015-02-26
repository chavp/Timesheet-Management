using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using Cwn.PM.BusinessModels.Values;
    using FluentNHibernate.Mapping;

    public class InitialMemberMap
        : EntityMap<InitialMember>
    {
        public InitialMemberMap()
            : base("PRJ_INIT")
        {
            Table("PRJ_INITIAL_MEMBER");

            Map(x => x.GroupName, "GROUP_NAME").Length(100).Not.Nullable();

            References(x => x.User, "AUT_ID").Not.Nullable().UniqueKey("KEY_PRJ_INITIAL_MEMBER");
            References(x => x.ProjectRole, "PJ_ROLE_ID").Not.Nullable().UniqueKey("KEY_PRJ_INITIAL_MEMBER");

            MapVersion();
        }
    }
}
