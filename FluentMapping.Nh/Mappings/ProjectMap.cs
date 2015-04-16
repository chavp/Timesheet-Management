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
            Map(x => x.NameEN, "PJ_NAME_EN").Not.Nullable();
            Map(x => x.NameTH, "PJ_NAME_TH").Not.Nullable();
            Map(x => x.StartDate, "START_DATE").CustomType("Date");
            Map(x => x.EndDate, "END_DATE").CustomType("Date");
            Map(x => x.CustomerName, "CUSTOMER_NAME");
            Map(x => x.Logo, "PJ_LOGO");

            Map(x => x.ContractStartDate, "CONTRACT_START_DATE").CustomType("Date");
            Map(x => x.ContractEndDate, "CONTRACT_END_DATE").CustomType("Date");
            Map(x => x.DeliverDate, "DELIVER_DATE").CustomType("Date");
            Map(x => x.WarrantyStartDate, "WARRANTY_START_DATE").CustomType("Date");
            Map(x => x.WarrantyEndDate, "WARRANTY_END_DATE").CustomType("Date");
            Map(x => x.EstimateProjectValue, "ESTIMATE_PROJECT_VALUE").CustomSqlType("NUMERIC(13, 2)");
            Map(x => x.ProjectValue, "PROJECT_VALUE").CustomSqlType("NUMERIC(13, 2)");

            Map(x => x.IsNonProject, "PJ_NONPROJECT_FLAG");

            //Map(x => x.Progress, "PJ_PROGRESS");

            HasMany(x => x.Members)
                .KeyColumn("PJ_ID")
                .Cascade
                .All()
                .Inverse()
                .LazyLoad();

            HasMany(x => x.TimeSheets)
                .KeyColumn("PJ_ID")
                .Cascade
                .All()
                .LazyLoad();

            References(x => x.Status, "PRJ_STATUS_ID");
            References(x => x.Customer, "CUS_ID");

            //References(x => x.Progress, "PRJ_PROGRESS_ID");

            MapVersion();
        }
    }
}
