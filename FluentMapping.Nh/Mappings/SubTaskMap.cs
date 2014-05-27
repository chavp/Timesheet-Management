using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class SubTaskMap
        : EntityMap<SubTask>
    {
        public SubTaskMap()
        {
            Table("SubTasks");

            Map(x => x.Name).Unique();
        }
    }
}
