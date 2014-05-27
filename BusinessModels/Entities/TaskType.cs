using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class TaskType
        : Entity
    {
        public virtual string NameEN { get; set; }
        public virtual string NameTH { get; set; }
        public virtual int Order { get; set; }
    }
}
