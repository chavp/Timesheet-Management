using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class PhaseMap
        : EntityMap<Phase>
    {
        public PhaseMap()
            : base("PJ_PHASE")
        {
            Table("PRJ_PROJECT_PHASE");

            Map(x => x.NameEN, "PJ_PHASE_NAME_EN").Length(100).Not.Nullable().Unique();
            Map(x => x.NameTH, "PJ_PHASE_NAME_TH").Length(100).Not.Nullable().Unique();
            Map(x => x.Order, "PJ_PHASE_ORDER");

            MapVersion();
        }
    }
}
