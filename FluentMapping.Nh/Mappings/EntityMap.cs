using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using FluentNHibernate.Mapping;

    public abstract class EntityMap<T>
        : ClassMap<T> where T : Cwn.PM.BusinessModels.Entities.Entity
    {
        public EntityMap(string prefix)
        {
            Id(t => t.ID, prefix + "_ID").GeneratedBy.Identity();
        }

        protected void MapVersion()
        {
            Map(t => t.CreatedAt, "CREATION_DTM").Not.Nullable();
            Map(t => t.CreatedBy, "CREATION_BY").Length(50).Not.Nullable();

            Version(t => t.ModifiedOn).CustomType("Timestamp").Column("UPDATE_DTM");
            Map(t => t.ModifiedBy, "UPDATE_BY").Length(50);
        }
    }
}
