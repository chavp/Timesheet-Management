using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;

    public class AppUserMap
        : EntityMap<AppUser>
    {
        public AppUserMap()
            : base("AUT_LOGIN")
        {
            Table("AUT_LOGIN_USER");

            Map(x => x.LoginName, "LOGIN_NAME").Not.Nullable().Unique();

            References(x => x.RefUser, "AUT_ID").Not.Nullable().Unique();

            MapVersion();
        }
    }
}
