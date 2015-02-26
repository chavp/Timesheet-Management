using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class Customer : Entity
    {
        protected Customer() { }
        public Customer(string name)
            : this()
        {
            Name = name;
        }

        public virtual string Name { get; set; }
        public virtual string ContactChannel { get; set; }

        public virtual Industry Industry { get; set; }
    }
}
