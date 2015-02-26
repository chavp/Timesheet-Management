using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class PositionMap
        : EntityMap<Position>
    {
        public PositionMap()
            : base("POSITION")
        {
            Table("MST_POSITION");

            Map(x => x.NameEN, "POSITION_NAME_EN").Length(100).Not.Nullable().Unique();
            Map(x => x.NameTH, "POSITION_NAME_TH").Length(100).Not.Nullable().Unique();
            Map(x => x.NameAbbrEN, "POSITION_NAME_ABBR_EN").Length(50);
            Map(x => x.NameAbbrTH, "POSITION_NAME_ABBR_TH").Length(50);

            Map(x => x.Cost, "RATE_PER_DAY").CustomSqlType("NUMERIC(9, 2)");
            //References(x => x.Department, "DEPARTMENT_ID");

            MapVersion();
        }
    }
}
