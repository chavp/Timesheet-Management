using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class Phase
        : Entity
    {
        public Phase()
        {
            
        }

        public virtual string NameEN { get; set; }
        public virtual string NameTH { get; set; }
        public virtual int Order { get; set; }
    }
}
