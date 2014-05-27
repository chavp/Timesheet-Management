using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class OrganizationMap
        : EntityMap<Organization>
    {
        public OrganizationMap()
            : base("ORG")
        {
            Table("MST_ORGANIZATION");

            Map(x => x.Name, "ORG_NAME").Length(100).UniqueKey("KEY_ORG_NAME");
            Map(x => x.ShortName, "ORG_SHORT_NAME").Length(100).UniqueKey("KEY_ORG_NAME");

            HasMany(x => x.Divisions)
                .KeyColumn("ORG_ID")
                .Cascade
                .All();

            MapVersion();
        }
    }
}
