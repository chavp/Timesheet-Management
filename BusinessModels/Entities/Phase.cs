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
        protected Phase() { }
        public Phase(string name, int order) 
            : this()
        {
            NameEN = name;
            NameTH = name;
            Order = order;
        }

        public virtual string NameEN { get; set; }
        public virtual string NameTH { get; set; }
        public virtual int Order { get; set; }

        public virtual void ChangeNameOrOrder(string name, int order)
        {
            NameEN = name;
            NameTH = name;
            Order = order;
        }
    }
}
