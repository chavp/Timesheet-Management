using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class ProjectMember
        : Entity
    {
        public virtual Project Project { get; set; }
        public virtual ProjectRole ProjectRole { get; set; }
        public virtual User User { get; set; }


    }
}
