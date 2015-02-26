using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;

    public class IndustryMap
        : EntityMap<Industry>
    {
        public IndustryMap()
            : base("INDUSTRY")
        {
            Table("BU_INDUSTRY");

            Map(x => x.Code, "CODE").Length(20).Not.Nullable();
            Map(x => x.Name, "NAME").Length(150).Not.Nullable();
            Map(x => x.NameTH, "NAME_TH").Length(150);

            MapVersion();
        }
    }
}
