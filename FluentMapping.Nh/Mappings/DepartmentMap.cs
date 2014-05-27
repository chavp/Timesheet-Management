using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class DepartmentMap
        : EntityMap<Department>
    {
        public DepartmentMap()
            : base("DEPT")
        {
            Table("MST_DEPARTMENT");

            Map(x => x.NameEN, "DEPT_NAME_EN").Length(100).Not.Nullable();
            Map(x => x.NameTH, "DEPT_NAME_TH").Length(100).Not.Nullable();

            References(x => x.Division, "DIVISION_ID");

            MapVersion();
        }
    }
}
