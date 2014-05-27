using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class MainTask : Entity
    {
        public MainTask()
        {
            
        }

        public virtual string Desc { get; set; }
    }
}
