using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class CustomerMap
        : EntityMap<Customer>
    {
        public CustomerMap()
            : base("CUS")
        {
            Table("PRJ_CUSTOMER");

            Map(x => x.Name, "CUS_NAME");
            Map(x => x.ContactChannel, "CUS_CONTACT_CHANNEL").CustomSqlType("NVARCHAR(MAX)");

            References(x => x.Industry, "INDUSTRY_ID");

            MapVersion();
        }
    }
}
