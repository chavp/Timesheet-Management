using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Orm.Loggers.Mappings
{
    using FluentNHibernate.Mapping;

    public abstract class LogMap<T>
        : ClassMap<T> where T : Cwn.PM.Loggers.Log
    {
        public LogMap()
        {
            Id(t => t.ID).GeneratedBy.Guid();

            Version(t => t.Version);
            Map(t => t.EventDate);
            Map(t => t.UserName);
            Map(t => t.Name);
            Map(t => t.Message).CustomSqlType("text");
            Map(t => t.Level);
        }

    }
}
