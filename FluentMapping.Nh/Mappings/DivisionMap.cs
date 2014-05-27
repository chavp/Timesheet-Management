using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class DivisionMap
        : EntityMap<Division>
    {
        public DivisionMap()
            : base("DIVISION")
        {
            Table("MST_DIVISION");

            Map(x => x.NameEN, "DIVISION_NAME_EN").Length(100).Not.Nullable().Unique();
            Map(x => x.NameTH, "DIVISION_NAME_TH").Length(100).Unique();

            References(x => x.Organization, "ORG_ID");

            HasMany(x => x.Departments)
                .Cascade
                .All();

            MapVersion();
        }
    }
}
