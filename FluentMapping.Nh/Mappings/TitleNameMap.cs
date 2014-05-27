using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;

    public class TitleNameMap
        : EntityMap<TitleName>
    {
        public TitleNameMap()
            : base("TITLE")
        {
            Table("MST_TITLE_NAME");

            Map(x => x.NameEN, "TITLE_NAME_EN").Length(100).Not.Nullable().Unique();
            Map(x => x.NameTH, "TITLE_NAME_TH").Length(100).Not.Nullable().Unique();

            MapVersion();
        }
    }
}
