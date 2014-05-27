using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Orm.Loggers.Mappings
{
    using Cwn.PM.Loggers;
    using FluentNHibernate.Mapping;

    public class ErrorLogMap
        : LogMap<ErrorLog>
    {
        public ErrorLogMap()
        {
            Table("ErrorLog");

            Map(x => x.ErrorMessage).CustomSqlType("text");
            Map(x => x.StackTrace).CustomSqlType("text");
        }
    }
}
