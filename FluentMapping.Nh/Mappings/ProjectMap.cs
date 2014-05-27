using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;
    using FluentNHibernate.Mapping;

    public class ProjectMap
        : EntityMap<Project>
    {
        public ProjectMap()
            : base("PJ")
        {
            Table("PRJ_PROJECT");

            Map(x => x.Code, "PJ_CODE").Length(50).Not.Nullable().Unique();
            Map(x => x.NameEN, "PJ_NAME_EN");
            Map(x => x.NameTH, "PJ_NAME_TH");
            Map(x => x.StartDate, "START_DATE").CustomType("Date");
            Map(x => x.EndDate, "END_DATE").CustomType("Date");
            Map(x => x.Logo, "PJ_LOGO");

            HasMany(x => x.Members)
                .KeyColumn("PJ_ID")
                .Cascade
                .All()
                .LazyLoad();

            HasMany(x => x.TimeSheets)
                .KeyColumn("PJ_ID")
                .Cascade
                .All()
                .LazyLoad();

            MapVersion();
        }
    }
}
