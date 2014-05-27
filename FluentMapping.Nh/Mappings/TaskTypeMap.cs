using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class TaskTypeMap
        : EntityMap<TaskType>
    {
        public TaskTypeMap()
            : base("TASK_TYPE")
        {
            Table("PRJ_TASK_TYPE");

            Map(x => x.NameEN, "TASK_TYPE_NAME_EN").Length(100).Not.Nullable().Unique();
            Map(x => x.NameTH, "TASK_TYPE_NAME_TH").Length(100).Not.Nullable().Unique();
            Map(x => x.Order, "TASK_TYPE_ORDER");

            MapVersion();
        }
    }
}
