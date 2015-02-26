using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using Cwn.PM.BusinessModels.Values;
    using FluentNHibernate.Mapping;

    public class ProjectProgressUpdateLogMap
        : BigEntityMap<ProjectProgressUpdateLog>
    {
        public ProjectProgressUpdateLogMap()
            : base("PJ_PRG")
        {
            Table("PRJ_PROJECT_PROGRESS_UPDATE_LOG");

            References(x => x.Project, "PJ_ID").Not.Nullable().UniqueKey("KEY_PROJECT_PROGRESS_UDATED");
            Map(x => x.UpdatedDate, "PROJECT_PROGRESS_UDATED_DATE").CustomType("Date").Not.Nullable().UniqueKey("KEY_PROJECT_PROGRESS_UDATED");
            
            MapVersion();        
        }
    }
}
