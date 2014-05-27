using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class Organization
        : Entity
    {
        public Organization()
        {
            Divisions = new List<Division>();
        }

        public virtual string Name { get; set; }
        public virtual string ShortName { get; set; }
       
        public virtual IList<Division> Divisions { get; protected set; }
    }
}
