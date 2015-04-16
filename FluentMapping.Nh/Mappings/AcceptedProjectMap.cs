using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class AcceptedProjectMap
        : EntityMap<AcceptedProject>
    {
        public AcceptedProjectMap()
            : base("PJ_ACCEPTED_PROJECT")
        {
            Table("PRJ_ACCEPTED_PROJECT");

            References(x => x.AcceptBy, "AUT_ID").Not.Nullable().UniqueKey("KEY_ACCEPTED_PROJECT");
            References(x => x.Project, "PJ_ID").Not.Nullable().UniqueKey("KEY_ACCEPTED_PROJECT");
            
            MapVersion();
        }
    }
}
