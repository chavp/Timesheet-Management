using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public abstract class Entity
    {
        public Entity()
        {
            CreatedAt = DateTime.Now;
            CreatedBy = Environment.UserName;
            ModifiedBy = Environment.UserName;
        }

        public virtual long ID { get; protected set; }

        public virtual DateTime CreatedAt { get; protected set; }
        public virtual string CreatedBy { get; set; }

        public virtual DateTime? ModifiedOn { get; protected set; }
        public virtual string ModifiedBy { get; set; }
    }
}
