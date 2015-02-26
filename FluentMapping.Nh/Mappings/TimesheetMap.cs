using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class TimesheetMap
        : BigEntityMap<Timesheet>
    {
        public TimesheetMap()
            : base("TIMESHEET")
        {
            Table("TME_TIMESHEET");

            References(x => x.ActualUserPosition, "POSITION_ID").Not.Nullable();
            References(x => x.ActualUserDepartment, "DEPT_ID").Not.Nullable();

            Map(x => x.ActualStartDate, "TASK_DATE").CustomType("Date");
            Map(x => x.ActualHourUsed, "HOUR_USED").CustomSqlType("NUMERIC(7, 4)");
            Map(x => x.Remark, "REMARK");
            Map(x => x.MainTask, "MAIN_TASK_DESC").Not.Nullable();
            Map(x => x.SubTask, "SUB_TASK_DESC").CustomSqlType("NVARCHAR(MAX)");
            Map(x => x.IsOT, "OT_FLAG");

            References(x => x.Project, "PJ_ID").Not.Nullable();
            References(x => x.ProjectRole, "PJ_ROLE_ID").Not.Nullable();
            References(x => x.User, "AUT_ID").Not.Nullable();

            References(x => x.Phase, "PJ_PHASE_ID").Not.Nullable();
            
            References(x => x.TaskType, "TASK_TYPE_ID").Not.Nullable();

            MapVersion();
        }
    }
}
