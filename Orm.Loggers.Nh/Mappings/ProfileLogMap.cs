using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Orm.Loggers.Mappings
{
    using Cwn.PM.Loggers;
    using FluentNHibernate.Mapping;

    public class ProfileLogMap
        : LogMap<ProfileLog>
    {
        public ProfileLogMap()
        {
            Table("ProfileLog");

            Map(x => x.UserID);
            Map(x => x.Action);
            Map(x => x.Controller);
            Map(x => x.ElapsedTime);
            Map(x => x.UserHostAddress);
            Map(x => x.QueryString).CustomSqlType("text");
            Map(x => x.PostData).CustomSqlType("text");
        }
    }
}
