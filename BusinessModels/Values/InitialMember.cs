using Cwn.PM.BusinessModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Values
{
    public class InitialMember : Entity
    {
        public virtual string GroupName { get; set; }
        public virtual User User { get; set; }
        public virtual ProjectRole ProjectRole { get; set; }
    }
}
