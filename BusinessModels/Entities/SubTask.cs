using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class SubTask
        : Entity
    {
        public SubTask()
        {
            
        }

        public virtual string Name { get; set; }
    }
}
