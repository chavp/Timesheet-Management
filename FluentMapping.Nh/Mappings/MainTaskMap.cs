using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class MainTaskMap
        : EntityMap<MainTask>
    {
        public MainTaskMap()
            : base("MAIN_TASK")
        {
            Table("PRJ_MAIN_TASK");

            Map(x => x.Desc, "MAIN_TASK_DESC").Unique();

            MapVersion();
        }
    }
}
