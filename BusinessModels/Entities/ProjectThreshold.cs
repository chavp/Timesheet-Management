using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class ProjectThreshold
        : Entity
    {
        public virtual string Name { get; set; }
        public virtual decimal LimitRatio { get; set; }
    }
}
