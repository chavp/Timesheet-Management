using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class MainTask : Entity
    {
        protected MainTask() { }

        public MainTask(string desc)
            : this()
        {
            Desc = desc;
        }

        public virtual string Desc { get; set; }
    }
}
