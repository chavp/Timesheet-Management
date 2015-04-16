using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;

    public class ProjectDeliveryPhaseMap
        : EntityMap<ProjectDeliveryPhase>
    {
        public ProjectDeliveryPhaseMap()
            : base("PRJ_DELIVERY_PHASE")
        {
            Table("PRJ_DELIVERY_PHASE");

            References(x => x.Project, "PJ_ID")
                .Not.Nullable().UniqueKey("KEY_PRJ_DELIVERY_PHASE");
            Map(x => x.DeliveryPhaseDate, "PJ_DELIVERY_PHASE_DATE").CustomType("Date")
                .Not.Nullable().UniqueKey("KEY_PRJ_DELIVERY_PHASE");

            Map(x => x.StatusOfProjectDeliveryPhase, "STATE_OF_PRJ_DELIVERY_PHASE")
                .Length(50);

            MapVersion();
        }
    }
}
