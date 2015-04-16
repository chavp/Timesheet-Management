using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;

    public class ProjectProgressEntryMap
        : EntityMap<ProjectProgressEntry>
    {
        public ProjectProgressEntryMap()
            : base("PRJ_PROGRESS_ENTRY")
        {
            Table("PRJ_PROGRESS_ENTRY");

            References(x => x.ProjectProgress, "PRJ_PROGRESS_ID").Not.Nullable();

            Map(x => x.LastUpdateDate, "LAST_UPDATE_DATE").CustomType("Date");
            Map(x => x.Amount, "AMOUNT").CustomSqlType("NUMERIC(5, 0)");
            Map(x => x.Note, "NOTE").CustomSqlType("NVARCHAR(255)");
            Map(x => x.StateOfProgress, "STATE_OF_PROGRESS").Length(50);

            MapVersion();
        }
    }
}
