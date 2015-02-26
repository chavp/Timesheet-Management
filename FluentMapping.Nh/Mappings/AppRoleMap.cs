using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class AppRoleMap
        : EntityMap<AppRole>
    {
        public AppRoleMap()
            : base("APP_ROLE")
        {
            Table("AUT_APP_ROLE");

            Map(x => x.Name, "APP_ROLE_NAME").Length(100).Not.Nullable().Unique();

            HasManyToMany(x => x.Users).Table("AUT_USERS_TO_APP_ROLE")
                .ParentKeyColumn("APP_ROLE_ID")
                .ChildKeyColumn("AUT_ID");

            HasManyToMany(x => x.AppUsers).Table("AUT_LOGIN_USERS_TO_APP_ROLE")
                .ParentKeyColumn("APP_ROLE_ID")
                .ChildKeyColumn("AUT_LOGIN_ID");

            MapVersion();
        }
    }
}
