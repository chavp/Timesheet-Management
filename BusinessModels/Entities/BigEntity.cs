using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public abstract class BigEntity
    {
        public BigEntity()
        {
            CreatedAt = DateTime.Now;
            CreatedBy = Environment.UserName;
            ModifiedBy = Environment.UserName;
        }

        public virtual Guid ID { get; protected set; }

        public virtual DateTime CreatedAt { get; protected set; }
        public virtual string CreatedBy { get; set; }

        public virtual DateTime? ModifiedOn { get; protected set; }
        public virtual string ModifiedBy { get; set; }
    }
}
